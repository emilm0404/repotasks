using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _db;

    public BookService(LibraryDbContext db) => _db = db;

    public async Task<IReadOnlyList<MostBorrowedBookDto>> GetMostBorrowedBooksAsync(int take = 10, CancellationToken ct = default)
    {
        take = take <= 0 ? 10 : Math.Min(take, 100);

        return await _db.Loans
            .GroupBy(l => new { l.Copy.BookId, l.Copy.Book.Title, l.Copy.Book.Author })
            .Select(g => new MostBorrowedBookDto
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                Author = g.Key.Author,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .ThenBy(b => b.Title)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<AvailabilityDto?> GetAvailabilityAsync(int bookId, CancellationToken ct = default)
    {
        return await _db.Books
            .Where(b => b.Id == bookId)
            .Select(b => new AvailabilityDto
            {
                BookId = b.Id,
                Title = b.Title,
                TotalCopies = b.Copies.Count(),
                Borrowed = b.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null)
            })
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<CoBorrowedBookDto>> GetCoBorrowedBooksAsync(int bookId, CancellationToken ct = default)
    {
        var borrowerIdsQuery = _db.Loans
            .Where(l => l.Copy.BookId == bookId)
            .Select(l => l.UserId)
            .Distinct();

        if (!await borrowerIdsQuery.AnyAsync(ct))
        {
            return Array.Empty<CoBorrowedBookDto>();
        }

        return await _db.Loans
            .Where(l => l.Copy.BookId != bookId && borrowerIdsQuery.Contains(l.UserId))
            .GroupBy(l => new { l.Copy.BookId, l.Copy.Book.Title })
            .Select(g => new CoBorrowedBookDto
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                SharedBorrowerCount = g.Select(x => x.UserId).Distinct().Count(),
                LoanCount = g.Count()
            })
            .OrderByDescending(b => b.SharedBorrowerCount)
            .ThenByDescending(b => b.LoanCount)
            .ThenBy(b => b.Title)
            .ToListAsync(ct);
    }

    public async Task<ReadRateDto?> GetReadRateAsync(int bookId, CancellationToken ct = default)
    {
        var bookInfo = await _db.Books
            .Where(b => b.Id == bookId)
            .Select(b => new { b.Id, b.Title, b.PageCount })
            .SingleOrDefaultAsync(ct);

        if (bookInfo is null)
        {
            return null;
        }

        var completedLoans = await _db.Loans
            .Where(l => l.Copy.BookId == bookId && l.ReturnedAt != null)
            .Select(l => new { l.BorrowedAt, l.ReturnedAt })
            .ToListAsync(ct);

        if (completedLoans.Count == 0)
        {
            return new ReadRateDto
            {
                BookId = bookInfo.Id,
                Title = bookInfo.Title,
                PageCount = bookInfo.PageCount,
                AveragePagesPerDay = 0,
                CompletedLoans = 0
            };
        }

        var totalRate = completedLoans.Sum(loan =>
        {
            var duration = (loan.ReturnedAt!.Value - loan.BorrowedAt).TotalDays;
            duration = duration <= 0 ? 1 : duration;
            return bookInfo.PageCount / duration;
        });

        return new ReadRateDto
        {
            BookId = bookInfo.Id,
            Title = bookInfo.Title,
            PageCount = bookInfo.PageCount,
            CompletedLoans = completedLoans.Count,
            AveragePagesPerDay = Math.Round(totalRate / completedLoans.Count, 2)
        };
    }
}
