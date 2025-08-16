using PoliceDataIngest.Services;

Console.WriteLine("Getting all documented street crimes since the start of 2025");

var file = await ApiService.DownloadZip(2025, 06);
Console.WriteLine("Processing dataset archive");
var crimes = ParseService.ParseZip(file, 2025, 0);

file.Delete();