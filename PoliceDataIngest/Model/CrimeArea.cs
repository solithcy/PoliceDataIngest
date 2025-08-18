using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PoliceDataIngest.Model;

[Table("crime_areas")]
[PrimaryKey(nameof(Date), nameof(H3))]
public class CrimeArea
{
    public CrimeArea(ulong h3, DateTime date)
    {
        H3 = h3;
        Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
    }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("h3", TypeName = "bigint")]
    public ulong H3 { get; set; }

    [Column("weapon_crime", TypeName = "int")]
    public uint WeaponCrime { get; set; }
    
    [Column("personal_theft", TypeName = "int")]
    public uint PersonalTheft { get; set; }
    
    [Column("bicycle_theft", TypeName = "int")]
    public uint BicycleTheft { get; set; }
    
    [Column("burglary", TypeName = "int")]
    public uint Burglary { get; set; }
    
    [Column("robbery", TypeName = "int")]
    public uint Robbery { get; set; }
    
    [Column("violent", TypeName = "int")]
    public uint Violent { get; set; }
    
    [Column("damage", TypeName = "int")]
    public uint Damage { get; set; }
    
    [Column("shoplifting", TypeName = "int")]
    public uint Shoplifting { get; set; }
    
    [Column("anti_social", TypeName = "int")]
    public uint AntiSocial { get; set; }
    
    [Column("vehicle_crime", TypeName = "int")]
    public uint VehicleCrime { get; set; }
    
    [Column("drugs", TypeName = "int")]
    public uint Drugs { get; set; }

    public int CalculateHashCode()
    {
        return HashCode.Combine(H3, Date);
    }

    public static int CalculateHashCode(ulong h3, DateTime date)
    {
        return HashCode.Combine(h3, date);
    }
}