using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliceDataIngest.Model;

[Table("crime_areas")]
public class CrimeArea
{
    public CrimeArea(ulong h3, DateTime date)
    {
        H3 = h3;
        Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
    }
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("h3", TypeName = "bigint")]
    public ulong H3 { get; set; }

    [Column("weapon_crime")]
    public uint WeaponCrime { get; set; }
    
    [Column("personal_theft")]
    public uint PersonalTheft { get; set; }
    
    [Column("bicycle_theft")]
    public uint BicycleTheft { get; set; }
    
    [Column("burglary")]
    public uint Burglary { get; set; }
    
    [Column("robbery")]
    public uint Robbery { get; set; }
    
    [Column("violent")]
    public uint Violent { get; set; }
    
    [Column("damage")]
    public uint Damage { get; set; }
    
    [Column("shoplifting")]
    public uint Shoplifting { get; set; }

    public int CalculateHashCode()
    {
        return HashCode.Combine(H3, Date);
    }

    public static int CalculateHashCode(ulong h3, DateTime date)
    {
        return HashCode.Combine(h3, date);
    }
}