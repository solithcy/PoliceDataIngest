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
var existing = await context.CrimeAreas.ToDictionaryAsync(area => area.CalculateHashCode(), area => area);

foreach (var crime in crimeParser.GetCrimes())
{
    var crimeHashCode = CrimeArea.CalculateHashCode(crime.H3Index, crime.Date);
    if (!existing.TryGetValue(crimeHashCode, out var ca))
    {
        ca = new CrimeArea(crime.H3Index, crime.Date);
        context.CrimeAreas.Add(ca);
        existing[ca.CalculateHashCode()] = ca;
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

Console.WriteLine("Saving changes");
await context.SaveChangesAsync();