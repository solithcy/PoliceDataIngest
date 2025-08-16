using PoliceDataIngest.Services;

namespace PoliceDataIngest.Model;

public class Crime
{
    [CsvColumn("Crime ID", typeof(string))]
    public string Id { get; set; }
    
    [CsvColumn("Month", typeof(DateTime), Format="yyyy-MM")]
    public DateTime Date { get; set; }
    
    [CsvColumn("Longitude", typeof(double))]
    public double Longitude { get; set; }
    
    [CsvColumn("Latitude", typeof(double))]
    public double Latitude { get; set; }
    
    [CsvColumn("Crime type", typeof(string))]
    public string CrimeType { get; set; }
    
    [CsvColumn("Reported by", typeof(string))]
    public string ReportedBy { get; set; }

    public ulong H3Index;
}