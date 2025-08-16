using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using H3;
using NetTopologySuite.Geometries;
using PoliceDataIngest.Model;

namespace PoliceDataIngest.Services;

public static class ParseService
{
    public static List<Crime> ParseZip(FileInfo file, int year, int month, bool exact = false)
    {
        ConcurrentBag<Crime> crimes = [];
        ZipArchive zip = ZipFile.OpenRead(file.FullName);

        Console.WriteLine("Filtering & batching dataset files");
        const int batchSize = 16;
        var batches = zip.Entries.Where(entry =>
        {
            if (!entry.FullName.EndsWith("-street.csv")) return false;
            int fileYear = int.Parse(entry.FullName[..4]);
            int fileMonth = int.Parse(entry.FullName[5..7]);
            if (exact) return fileYear == year && fileMonth == month;
            return year <= fileYear && (year != fileYear || month <= fileMonth);
        }).Chunk(batchSize);

        Console.WriteLine($"{batches.Count()} batches of {batchSize}, {batches.Sum(b=>b.Length)} total entries ({(batches.Sum(b => b.Sum(e => e.Length)) / 1024 / 1024):F2}MB)");

        int fidx = 0;
        foreach (var batch in batches)
        {
            Console.Write($"\rProcessing files {fidx+1}-{Math.Min(fidx + batchSize, fidx + batch.Length)}");
            fidx += batch.Length;
            batch.Select(entry =>
            {
                var stream = entry.Open();
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }).AsParallel().WithDegreeOfParallelism(16).ForAll(ms =>
            { 
                var reader = new StreamReader(ms);
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
                        try
                        {
                            Crime c = DeserializeRow(cols, parts);
                            if (!CrimeTypes.IsCrimeType(c.CrimeType)) continue;
                            index = H3Index.FromPoint(new Point(c.Longitude, c.Latitude), 10);
                            c.H3Index = (ulong) index;
                            crimes.Add(c);
                        }
                        catch (FormatException)
                        {
                            // assume crime had no location, as many don't
                        }
                    }
                }

                reader.Dispose();
                ms.Dispose();
            });
        }

        zip.Dispose();
        
        var list = crimes.ToList();
        Console.WriteLine($"\nFinished processing dataset, {list.Count} entries in target range");

        return list;
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