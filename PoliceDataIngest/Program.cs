using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PoliceDataIngest.Context;
using PoliceDataIngest.Model;
using PoliceDataIngest.Services;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--police-data"))
        {
            var t = AddCrimeData();
            t.Wait();
        }
        else
        {
            Console.WriteLine("Skipping police data");
        }
        
        if (args.Contains("--pop-data"))
        {
            var t = AddPopulationData();
            t.Wait();
        }
        else
        {
            Console.WriteLine("Skipping population data");
        }
    }

    private static async Task AddCrimeData()
    {
        Console.WriteLine("Getting all documented street crimes");
        var file = await ApiService.DownloadZip(0, 0);
        Console.WriteLine("Connecting to database");
        var factory = new PoliceDbContextFactory();
        var context = factory.CreateDbContext([]);
        Console.WriteLine("Getting existing crime areas");
        var existing =  await context.CrimeAreas.Select(area => area.CalculateHashCode()).ToHashSetAsync();
        Console.WriteLine($"Parsing, filtering and writing crimes to database");
        var crimeParser = new ParseService(file, 0, 0);
        Dictionary<int, CrimeArea> newAreas = [];
        DateTime? currentDate = null;

        foreach (var crime in crimeParser.GetCrimes())
        {

            if (newAreas.Count >= 25000)
            {
                currentDate ??= crime.Date;
                if (currentDate != crime.Date)
                {
                    // enough stuff has aggregated and things in newAreas won't be mutated, push new areas
                    // i'm doing it this way because the memory usage from efcore is crazy
                    await context.QuickPushCrimeAreas(newAreas.Values.ToList());
                    newAreas = [];
                    currentDate = null;
                }
            }
            
            var crimeHashCode = CrimeArea.CalculateHashCode(crime.H3Index, crime.Date);
            if (existing.Contains(crimeHashCode)) continue;
            if (!newAreas.TryGetValue(crimeHashCode, out var ca))
            {
                ca = new CrimeArea(crime.H3Index, crime.Date);
                newAreas[ca.CalculateHashCode()] = ca;
            }
            
            _ = crime.CrimeType switch
            {
                CrimeTypes.WeaponCrime   => ca.WeaponCrime++,
                CrimeTypes.Burglary      => ca.Burglary++,
                CrimeTypes.PersonalTheft => ca.PersonalTheft++,
                CrimeTypes.BicycleTheft  => ca.BicycleTheft++,
                CrimeTypes.Robbery       => ca.Robbery++,
                CrimeTypes.Violent       => ca.Violent++,
                CrimeTypes.Damage        => ca.Damage++,
                CrimeTypes.Shoplifting   => ca.Shoplifting++,
                CrimeTypes.AntiSocial    => ca.AntiSocial++,
                CrimeTypes.Drugs         => ca.Drugs++,
                CrimeTypes.VehicleCrime  => ca.VehicleCrime++,
                _ => (uint) 0
            };
        }

        if (newAreas.Count > 0)
        {
            await context.QuickPushCrimeAreas(newAreas.Values.ToList());
        }

        file.Delete();
    }
    
    private static async Task AddPopulationData()
    {
        Console.WriteLine("Connecting to database");
        var factory = new PoliceDbContextFactory();
        var context = factory.CreateDbContext([]);
        Console.WriteLine("Getting existing population areas");
        var existing =  await context.PopulationAreas.Select(area => area.CalculateHashCode()).ToHashSetAsync();
        Console.WriteLine("Parsing population geotiff");
        var pops = PopService.ReadPopulations();
        Console.WriteLine("Filtering existing entries");
        pops = pops.Where(pa => !existing.Contains(pa.CalculateHashCode())).ToList();
        Console.WriteLine("Writing to database");
        await context.QuickPushPopAreas(pops);
    }
}