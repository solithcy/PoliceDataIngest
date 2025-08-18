using System.Runtime.CompilerServices;

namespace PoliceDataIngest.Model;

public sealed class CrimeTypes
{
    public const string WeaponCrime = "Possession of weapons";
    public const string PersonalTheft = "Theft from the person";
    public const string BicycleTheft = "Bicycle theft";
    public const string Burglary = "Burglary";
    public const string Robbery = "Robbery";
    public const string Violent = "Violence and sexual offences";
    public const string Damage = "Criminal damage and arson";
    public const string Shoplifting = "Shoplifting";
    public const string AntiSocial = "Anti-social behaviour";
    public const string VehicleCrime = "Vehicle crime";
    public const string Drugs = "Drugs";
    
    private static readonly HashSet<string> All = new(
        [
            WeaponCrime, PersonalTheft, BicycleTheft, Burglary,
            Robbery, Violent, Damage, Shoplifting, AntiSocial,
            VehicleCrime, Drugs,
        ],
        StringComparer.Ordinal
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCrimeType(string? value) =>
        value is not null && All.Contains(value);
}