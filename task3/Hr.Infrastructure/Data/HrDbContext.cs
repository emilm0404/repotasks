using System;
using System.Linq;
using System.Security.Cryptography;
using Hr.Domain.Entities;
using Hr.Infrastructure.EntityConfigs;
using Microsoft.EntityFrameworkCore;

namespace Hr.Infrastructure.Data;

public class HrDbContext : DbContext
{
    public HrDbContext(DbContextOptions<HrDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var providerName = Database.ProviderName;
        var useManualConcurrency = providerName is not null && !providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);
        var employeeEntries = ChangeTracker.Entries<Employee>().ToList();

        if (useManualConcurrency)
        {
            foreach (var entry in employeeEntries.Where(e => e.State is EntityState.Modified or EntityState.Deleted))
            {
                var original = entry.Property(x => x.RowVersion).OriginalValue as byte[];
                if (original is null || original.Length == 0)
                {
                    throw new DbUpdateConcurrencyException("RowVersion is required for concurrency control.");
                }

                var current = await Employees.AsNoTracking()
                    .Where(e => e.Id == entry.Entity.Id)
                    .Select(e => e.RowVersion)
                    .SingleOrDefaultAsync(cancellationToken);

                if (current is null || !current.SequenceEqual(original))
                {
                    throw new DbUpdateConcurrencyException();
                }
            }
        }

        foreach (var entry in employeeEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.UpdatedAtUtc = utcNow;

                if (useManualConcurrency)
                {
                    entry.Property(x => x.RowVersion).CurrentValue = CreateRowVersion();
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;

                if (useManualConcurrency)
                {
                    entry.Property(x => x.RowVersion).CurrentValue = CreateRowVersion();
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static byte[] CreateRowVersion()
    {
        var buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        return buffer;
    }
}
