using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using H3;
using NetTopologySuite.Geometries;
using PoliceDataIngest.Model;

namespace PoliceDataIngest.Services;

public class ParseService
{
    private ZipArchive _archive;
    private Queue<ZipArchiveEntry> _entryTasks;
    private ConcurrentQueue<Crime> _crimes = [];
    private int _fidx = 0;
    private int _totalEntries;

    public ParseService(FileInfo file, int year, int month, bool exact = false)
    {
        _archive = ZipFile.OpenRead(file.FullName);

        Console.WriteLine("Filtering dataset files");

        var entries = _archive.Entries.Where(entry =>
        {
            if (!entry.FullName.EndsWith("-street.csv")) return false;
            int fileYear = int.Parse(entry.FullName[..4]);
            int fileMonth = int.Parse(entry.FullName[5..7]);
            if (exact) return fileYear == year && fileMonth == month;
            return year <= fileYear && (year != fileYear || month <= fileMonth);
        }).ToList();
        _entryTasks = new Queue<ZipArchiveEntry>(entries);
        _totalEntries = _entryTasks.Count;

        Console.WriteLine(
            $"{entries.Count} total entries ({(entries.Sum(e => e.Length) / 1024 / 1024):F2}MB)");
    }

    public IEnumerable<Crime> GetCrimes()
    {
        while (_entryTasks.TryDequeue(out var entry))
        {
            Console.Write($"\rProcessing file {++_fidx}/{_totalEntries}");
                var stream = entry.Open();
                var reader = new StreamReader(stream);
                string[]? cols = null;

                // having a shared H3Index variable should help with memory issues.. maybe
                H3Index index;
                while (reader.ReadLine() is { } line)
                {
                    string[] parts = line.Split(",");
                    if (cols is null)
                    {
                        cols = parts;
                    }
                    else
                    {
                        if (parts.Length != cols.Length) continue;
                        Crime c;
                        try
                        {
                            c = DeserializeRow(cols, parts);
                            if (!CrimeTypes.IsCrimeType(c.CrimeType)) continue;
                            index = H3Index.FromPoint(new Point(c.Longitude, c.Latitude), 10);
                            c.H3Index = (ulong) index;
                        }
                        catch (FormatException)
                        {
                            // assume crime had no location, as many don't
                            continue;
                        }

                        yield return c;
                    }
                }

                reader.Dispose();
                stream.Dispose();
        }

        _archive.Dispose();
    }


    private static Crime DeserializeRow(string[] cols, string[] parts)
    {
        object c = new Crime();

        var properties = typeof(Crime).GetProperties();

        foreach (var property in properties)
        {
            var csvAttribute = property.GetCustomAttribute<CsvColumn>();
            if (csvAttribute == null) continue;
            int valueIndex = Array.IndexOf(cols, csvAttribute.Name);
            if (valueIndex == -1) throw new UnhandledDeserializeType();
            string rawValue;
            try
            {
                rawValue = parts[valueIndex].Trim();
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("help");
                throw;
            }

            object convertedValue = csvAttribute.Type.Name switch
            {
                nameof(String) => rawValue,
                nameof(Int32) => int.Parse(rawValue),
                nameof(Double) => double.Parse(rawValue),
                nameof(DateTime) => DateTime.SpecifyKind(
                    DateTime.ParseExact(rawValue, csvAttribute.Format!, null),
                    DateTimeKind.Utc
                ),
                _ => throw new UnhandledDeserializeType()
            };
            property.SetValue(c, convertedValue, null);
        }

        return (Crime) c;
    }
}

public class UnhandledDeserializeType() : Exception("Unhandled csv type");

[AttributeUsage(AttributeTargets.Property)]
public class CsvColumn : Attribute
{
    public string Name { get; }
    public Type Type { get; }
    public string? Format { get; set; }

    public CsvColumn(string name, Type? type = null)
    {
        Name = name;
        Type = type ?? typeof(string);
    }
}