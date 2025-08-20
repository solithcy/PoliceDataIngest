using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;
using PoliceDataIngest.Model;

namespace PoliceDataIngest.Context;

public partial class PoliceDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public PoliceDbContext(DbContextOptions<PoliceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    public DbSet<CrimeArea> CrimeAreas { get; set; }
    public DbSet<PopulationArea> PopulationAreas { get; set; }
    
    public async Task QuickPushCrimeAreas(List<CrimeArea> areas)
    {
        var conn = (NpgsqlConnection)Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
        }

        await using var transaction = await conn.BeginTransactionAsync();

        {
            await using var writer = await conn.BeginBinaryImportAsync("COPY crime_areas (burglary, date, h3, personal_theft, weapon_crime, bicycle_theft, damage, robbery, shoplifting, violent, anti_social, drugs, vehicle_crime) FROM STDIN (FORMAT BINARY)");

            foreach (var ca in areas)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync((long)ca.Burglary, NpgsqlDbType.Integer);
                await writer.WriteAsync(ca.Date, NpgsqlDbType.TimestampTz);
                await writer.WriteAsync((long)ca.H3, NpgsqlDbType.Bigint);
                await writer.WriteAsync((long)ca.PersonalTheft, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.WeaponCrime, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.BicycleTheft, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.Damage, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.Robbery, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.Shoplifting, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.Violent, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.AntiSocial, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.Drugs, NpgsqlDbType.Integer);
                await writer.WriteAsync((long)ca.VehicleCrime, NpgsqlDbType.Integer);
            }

            await writer.CompleteAsync();   
        }

        await transaction.CommitAsync();
    }
    
    public async Task QuickPushPopAreas(List<PopulationArea> areas)
    {
        var conn = (NpgsqlConnection)Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
        }

        await using var transaction = await conn.BeginTransactionAsync();

        {
            await using var writer = await conn.BeginBinaryImportAsync("COPY pop_areas (h3, population) FROM STDIN (FORMAT BINARY)");

            foreach (var pa in areas)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync((long)pa.H3, NpgsqlDbType.Bigint);
                await writer.WriteAsync(pa.Population, NpgsqlDbType.Double);
            }

            await writer.CompleteAsync();   
        }

        await transaction.CommitAsync();
    }
}
