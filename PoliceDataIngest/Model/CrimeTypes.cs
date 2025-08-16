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
    
    private static readonly HashSet<string> All = new(
        [
            WeaponCrime, PersonalTheft, BicycleTheft, Burglary,
            Robbery, Violent, Damage, Shoplifting
        ],
        StringComparer.Ordinal
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCrimeType(string? value) =>
        value is not null && All.Contains(value);
}