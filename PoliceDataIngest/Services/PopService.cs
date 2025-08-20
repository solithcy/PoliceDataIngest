using System.Drawing;
using System.Reflection;
using H3;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OSR;
using PoliceDataIngest.Model;
using Point = NetTopologySuite.Geometries.Point;

namespace PoliceDataIngest.Services;

public static class PopService
{
    public static List<PopulationArea> ReadPopulations()
    {
        GdalBase.ConfigureAll();
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("PoliceDataIngest.Resources.uk_residential_population_2021.tif");
        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            stream!.CopyTo(ms);
            bytes = ms.ToArray();
        }

        string vsiPath = "/vsimem/uk_residential_population_2021.tif";
        Gdal.FileFromMemBuffer(vsiPath, bytes);

        using var ds = Gdal.Open(vsiPath, Access.GA_ReadOnly);
        if (ds == null)
            throw new Exception("GDAL could not open dataset");

        double[] geoTransform = new double[6];
        ds.GetGeoTransform(geoTransform);

        int width = ds.RasterXSize;
        int height = ds.RasterYSize;

        Band band = ds.GetRasterBand(1);
        double[] buffer = new double[width];

        var result = new Dictionary<ulong, double>();
        
        // dataset spatial reference is OSGB36 (northing, easting), we want world lat/lon (WGS84)
        SpatialReference srcSrs = new SpatialReference(ds.GetProjection());
        SpatialReference dstSrs = new SpatialReference("");
        dstSrs.ImportFromEPSG(4326); // WGS84
        CoordinateTransformation transform = new CoordinateTransformation(srcSrs, dstSrs);

        for (int row = 0; row < height; row++)
        {
            band.ReadRaster(0, row, width, 1, buffer, width, 1, 0, 0);

            for (int col = 0; col < width; col++)
            {
                double val = buffer[col];
                if (double.IsNaN(val)) continue;

                double x = geoTransform[0] + col * geoTransform[1] + row * geoTransform[2];
                double y = geoTransform[3] + col * geoTransform[4] + row * geoTransform[5];
                
                double[] point = [x, y, 0];
                transform.TransformPoint(point);
                double lat = point[0];
                double lon = point[1];
                
                ulong h3  = H3Index.FromPoint(new Point(lon, lat), 8);

                result[h3] = (result.GetValueOrDefault(h3, 0)) + val; 
            }
        }

        return result.Select(kvp =>
        {
            var p = new PopulationArea(kvp.Key)
            {
                Population = kvp.Value
            };
            return p;
        }).ToList();
    }
}