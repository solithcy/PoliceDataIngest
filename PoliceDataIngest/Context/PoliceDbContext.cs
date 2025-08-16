using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
}
