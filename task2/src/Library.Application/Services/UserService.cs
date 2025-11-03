using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Services;

public class UserService : IUserService
{
    private readonly LibraryDbContext _db;

    public UserService(LibraryDbContext db) => _db = db;

    public async Task<IReadOnlyList<TopBorrowerDto>> GetTopBorrowersAsync(DateTime from, DateTime to, int take = 10, CancellationToken ct = default)
    {
        (from, to) = NormalizeRange(from, to);
        take = take <= 0 ? 10 : Math.Min(take, 100);

        return await _db.Loans
            .Where(l => l.BorrowedAt >= from && l.BorrowedAt <= to)
            .GroupBy(l => new { l.UserId, l.User.FullName })
            .Select(g => new TopBorrowerDto
            {
                UserId = g.Key.UserId,
                UserName = g.Key.FullName,
                BorrowCount = g.Count(),
                From = from,
                To = to
            })
            .OrderByDescending(u => u.BorrowCount)
            .ThenBy(u => u.UserName)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BorrowHistoryGroupDto>> GetBorrowHistoryAsync(int userId, DateTime from, DateTime to, TimeGrouping grouping, CancellationToken ct = default)
    {
        (from, to) = NormalizeRange(from, to);

        var loans = await _db.Loans
            .Where(l => l.UserId == userId && l.BorrowedAt >= from && l.BorrowedAt <= to)
            .OrderBy(l => l.BorrowedAt)
            .Select(l => new BorrowHistoryItemDto
            {
                LoanId = l.Id,
                BookId = l.Copy.BookId,
                Title = l.Copy.Book.Title,
                BorrowedAt = l.BorrowedAt,
                ReturnedAt = l.ReturnedAt
            })
            .ToListAsync(ct);

        if (loans.Count == 0)
        {
            return Array.Empty<BorrowHistoryGroupDto>();
        }

        var groups = loans
            .GroupBy(item => GetPeriodKey(item.BorrowedAt, grouping))
            .OrderBy(g => g.Key.Sort)
            .Select(g => new BorrowHistoryGroupDto
            {
                Period = g.Key.Label,
                Loans = g
                    .OrderBy(l => l.BorrowedAt)
                    .ToList()
            })
            .ToList();

        return groups;
    }

    private static (DateTime Sort, string Label) GetPeriodKey(DateTime dateUtc, TimeGrouping grouping)
    {
        dateUtc = EnsureUtc(dateUtc);

        return grouping switch
        {
            TimeGrouping.Day => (Sort: dateUtc.Date, Label: dateUtc.ToString("yyyy-MM-dd")),
            TimeGrouping.Week => GetWeekKey(dateUtc),
            _ => (Sort: new DateTime(dateUtc.Year, dateUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc), Label: dateUtc.ToString("yyyy-MM"))
        };
    }

    private static (DateTime Sort, string Label) GetWeekKey(DateTime dateUtc)
    {
        var start = FirstDayOfWeek(dateUtc);
        return (Sort: start, Label: $"{start:yyyy-MM-dd} (week)");
    }

    private static DateTime FirstDayOfWeek(DateTime dateUtc)
    {
        var diff = (int)dateUtc.DayOfWeek;
        return dateUtc.Date.AddDays(-diff);
    }

    private static (DateTime from, DateTime to) NormalizeRange(DateTime from, DateTime to)
    {
        from = EnsureUtc(from).Date;
        to = EnsureUtc(to).Date;
        if (to < from)
        {
            (from, to) = (to, from);
        }

        to = to.Date.AddDays(1).AddTicks(-1);
        return (from, to);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => value
        };
    }
}
