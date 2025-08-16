using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using PoliceDataIngest.Model;

namespace PoliceDataIngest.Context;

public partial class PoliceDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public PoliceDbContext()
    {
    }

    public PoliceDbContext(DbContextOptions<PoliceDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=police;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    public DbSet<CrimeArea> CrimeAreas { get; set; }
    
    public async Task QuickPushCrimeAreas(List<CrimeArea> areas)
    {
        var conn = (NpgsqlConnection)Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
        }

        await using var transaction = await conn.BeginTransactionAsync();

        {
            await using var writer = await conn.BeginBinaryImportAsync("COPY crime_areas (burglary, date, h3, personal_theft, weapon_crime, bicycle_theft, damage, robbery, shoplifting, violent) FROM STDIN (FORMAT BINARY)");

            foreach (var ca in areas)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync((long)ca.Burglary, NpgsqlDbType.Bigint);
                await writer.WriteAsync(ca.Date, NpgsqlDbType.TimestampTz);
                await writer.WriteAsync((long)ca.H3, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.PersonalTheft, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.WeaponCrime, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.BicycleTheft, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.Damage, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.Robbery, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.Shoplifting, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.Violent, NpgsqlDbType.Bigint);
            }

            await writer.CompleteAsync();   
        }

        await transaction.CommitAsync();
    }
}
