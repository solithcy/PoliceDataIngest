namespace PoliceDataIngest.Services;

public static class ApiService
{
    public static async Task<FileInfo> DownloadZip(int year, int month)
    {
        string tempDir = Path.GetTempPath();
        string downloadUrl = $"https://data.police.uk/data/archive/{year:D4}-{month:D2}.zip";
        string filePath = Path.Join(tempDir, $"police-{year:D4}-{month:D2}.zip");
        
        var f = new FileInfo(filePath);
        if (f.Exists)
        {
            Console.WriteLine("File already exists, deleting");
            f.Delete();   
        }

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);
        
        using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        long? contentLength = response.Content.Headers.ContentLength;

        await using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var stream = await response.Content.ReadAsStreamAsync();
        
        var buffer = new byte[1024 * 1024];
        long totalRead = 0;
        int read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read));
            totalRead += read;

            double progress = (double)totalRead / contentLength!.Value * 100;
            Console.Write($"\rDownloading dataset... {totalRead / 1024.0 / 1024.0:F2}MB / {contentLength / 1024.0 / 1024.0:F2}MB ({progress:F2}%)");
        }

        Console.WriteLine("\nDownload complete");

        return new FileInfo(filePath);
    }
}