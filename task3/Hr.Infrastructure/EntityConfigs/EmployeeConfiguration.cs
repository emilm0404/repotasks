using Hr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hr.Infrastructure.EntityConfigs;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");
        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.EmployeeNumber).HasMaxLength(32).IsRequired();

        b.HasIndex(x => x.EmployeeNumber).IsUnique();

        b.Property(x => x.RowVersion).IsRowVersion();
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.UpdatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
    }
}