using Hr.Application.Abstractions;
using Hr.Application.DTOs;
using Hr.Domain.Entities;
using Hr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hr.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly HrDbContext _db;

    public EmployeeService(HrDbContext db) => _db = db;

    public async Task<PagedResult<EmployeeDto>> GetAsync(string? search, string? sort, string? dir, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        IQueryable<Employee> q = _db.Employees.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (tokens.Length == 0)
            {
                tokens = new[] { normalized };
            }

            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;
                var pattern = $"%{EscapeLike(token)}%";
                q = q.Where(e =>
                    EF.Functions.Like(e.FirstName, pattern, @"\") ||
                    EF.Functions.Like(e.LastName, pattern, @"\") ||
                    EF.Functions.Like(e.EmployeeNumber, pattern, @"\") ||
                    EF.Functions.Like(e.FirstName + " " + e.LastName, pattern, @"\") ||
                    EF.Functions.Like(e.LastName + " " + e.FirstName, pattern, @"\"));
            }
        }

        q = (sort?.ToLower(), (dir ?? "asc").ToLower()) switch
        {
            ("firstname", "desc") => q.OrderByDescending(x => x.FirstName).ThenBy(x => x.LastName),
            ("firstname", _) => q.OrderBy(x => x.FirstName).ThenBy(x => x.LastName),

            ("employeenumber", "desc") => q.OrderByDescending(x => x.EmployeeNumber),
            ("employeenumber", _) => q.OrderBy(x => x.EmployeeNumber),

            ("lastname", "desc") => q.OrderByDescending(x => x.LastName).ThenBy(x => x.FirstName),
            _ => q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                EmployeeNumber = e.EmployeeNumber,
                RowVersionBase64 = Convert.ToBase64String(e.RowVersion)
            })
            .ToListAsync(ct);

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            Sort = sort,
            Dir = dir
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var e = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            EmployeeNumber = e.EmployeeNumber,
            RowVersionBase64 = Convert.ToBase64String(e.RowVersion)
        };
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto, CancellationToken ct)
    {
        var normalizedFirst = dto.FirstName.Trim();
        var normalizedLast = dto.LastName.Trim();
        var normalizedEmployeeNumber = dto.EmployeeNumber.Trim().ToUpperInvariant();

        if (await _db.Employees.AnyAsync(x => x.EmployeeNumber == normalizedEmployeeNumber, ct))
            throw new InvalidOperationException("EmployeeNumber must be unique.");

        var e = new Employee
        {
            FirstName = normalizedFirst,
            LastName = normalizedLast,
            EmployeeNumber = normalizedEmployeeNumber
        };

        _db.Employees.Add(e);
        await _db.SaveChangesAsync(ct);

        return new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            EmployeeNumber = e.EmployeeNumber,
            RowVersionBase64 = Convert.ToBase64String(e.RowVersion)
        };
    }

    public async Task<EmployeeDto> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct)
    {
        var e = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) throw new KeyNotFoundException("Employee not found.");

        var original = ParseRowVersion(dto.RowVersionBase64);
        _db.Entry(e).Property(x => x.RowVersion).OriginalValue = original;

        var normalizedEmployeeNumber = dto.EmployeeNumber.Trim().ToUpperInvariant();
        if (!string.Equals(e.EmployeeNumber, normalizedEmployeeNumber, StringComparison.Ordinal))
        {
            var exists = await _db.Employees.AnyAsync(x => x.EmployeeNumber == normalizedEmployeeNumber && x.Id != id, ct);
            if (exists) throw new InvalidOperationException("EmployeeNumber must be unique.");
        }

        e.FirstName = dto.FirstName.Trim();
        e.LastName = dto.LastName.Trim();
        e.EmployeeNumber = normalizedEmployeeNumber;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Concurrency conflict. Reload the employee and retry.");
        }

        return new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            EmployeeNumber = e.EmployeeNumber,
            RowVersionBase64 = Convert.ToBase64String(e.RowVersion)
        };
    }

    public async Task DeleteAsync(int id, byte[] rowVersion, CancellationToken ct)
    {
        if (rowVersion is null || rowVersion.Length == 0)
            throw new InvalidOperationException("rowVersion is required.");

        var e = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return;

        _db.Entry(e).Property(x => x.RowVersion).OriginalValue = rowVersion;

        _db.Employees.Remove(e);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Concurrency conflict. Reload the employee and retry.");
        }
    }

    private static string EscapeLike(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal);
    }

    private static byte[] ParseRowVersion(string rowVersionBase64)
    {
        if (string.IsNullOrWhiteSpace(rowVersionBase64))
            throw new InvalidOperationException("rowVersionBase64 is required.");

        try
        {
            return Convert.FromBase64String(rowVersionBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("rowVersionBase64 must be valid base64.", ex);
        }
    }
}
