using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PoliceDataIngest.Model;

[Table("pop_areas")]
[PrimaryKey(nameof(H3))]
public class PopulationArea : IHashable
{
    public PopulationArea(ulong h3)
    {
        H3 = h3;
    }
    
    [Column("h3", TypeName = "bigint")]
    public ulong H3 { get; set; }

    [Column("population", TypeName = "double precision")]
    public double Population { get; set; }
    
    public int CalculateHashCode()
    {
        return HashCode.Combine(H3);
    }

    public static int CalculateHashCode(ulong h3)
    {
        return HashCode.Combine(h3);
    }
}