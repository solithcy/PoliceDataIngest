using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PoliceDataIngest.Context;
using PoliceDataIngest.Services;

public class PoliceDbContextFactory : IDesignTimeDbContextFactory<PoliceDbContext>
{
    public virtual PoliceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PoliceDbContext>();
        var connectionString = ConfigService.Load().GetConnectionString("Database")!;
        optionsBuilder.UseNpgsql(connectionString);

        return new PoliceDbContext(optionsBuilder.Options);
    }
}