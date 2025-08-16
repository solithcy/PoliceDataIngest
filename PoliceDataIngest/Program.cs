using Microsoft.EntityFrameworkCore;
using PoliceDataIngest.Context;
using PoliceDataIngest.Model;
using PoliceDataIngest.Services;

Console.WriteLine("Getting all documented street crimes");

var file = await ApiService.DownloadZip(2025, 06);
Console.WriteLine("Processing dataset archive");
Console.WriteLine($"Parsing, filtering and writing crimes to database");
var crimeParser = new ParseService(file, 0, 0);
var context = new PoliceDbContext();
var existing =  await context.CrimeAreas.Select(area => area.CalculateHashCode()).ToHashSetAsync();
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
        _ => (uint) 0
    };
}

if (newAreas.Count > 0)
{
    await context.QuickPushCrimeAreas(newAreas.Values.ToList());
}