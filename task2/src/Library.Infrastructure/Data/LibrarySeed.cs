using System.Linq;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data;

public static class LibrarySeed
{
    public static async Task EnsureSeedDataAsync(LibraryDbContext db, CancellationToken ct = default)
    {
        if (await db.Books.AnyAsync(ct))
        {
            return;
        }

        var books = new[]
        {
            new Book { Title = "Signals and Systems", Author = "Alan R. Oppenheim", PageCount = 560 },
            new Book { Title = "Clean Code", Author = "Robert C. Martin", PageCount = 464 },
            new Book { Title = "Cool Design", Author = "Eric Evans", PageCount = 560 },
            new Book { Title = "Design Patterns", Author = "Erich Gamma et al.", PageCount = 395 },
            new Book { Title = "The Cool Programmer", Author = "Andy Hunt & Dave Thomas", PageCount = 352 }
        };

        await db.Books.AddRangeAsync(books, ct);
        await db.SaveChangesAsync(ct);

        var signals = books.Single(b => b.Title == "Signals and Systems");
        var cleanCode = books.Single(b => b.Title == "Clean Code");
        var ddd = books.Single(b => b.Title == "Cool Design");
        var designPatterns = books.Single(b => b.Title == "Design Patterns");
        var pragmatic = books.Single(b => b.Title == "The Cool Programmer");

        var copies = new[]
        {
            new Copy { BookId = signals.Id },
            new Copy { BookId = signals.Id },
            new Copy { BookId = cleanCode.Id },
            new Copy { BookId = cleanCode.Id },
            new Copy { BookId = ddd.Id },
            new Copy { BookId = ddd.Id },
            new Copy { BookId = designPatterns.Id },
            new Copy { BookId = pragmatic.Id }
        };

        await db.Copies.AddRangeAsync(copies, ct);
        await db.SaveChangesAsync(ct);

        var users = new[]
        {
            new User { FullName = "Alice Johnson" },
            new User { FullName = "Bob Smith" },
            new User { FullName = "Charlie Nguyen" },
            new User { FullName = "Dana Williams" }
        };

        await db.Users.AddRangeAsync(users, ct);
        await db.SaveChangesAsync(ct);

        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var loans = new[]
        {
            new Loan { CopyId = copies[0].Id, UserId = users[0].Id, BorrowedAt = baseDate.AddDays(-60), ReturnedAt = baseDate.AddDays(-50) },
            new Loan { CopyId = copies[1].Id, UserId = users[1].Id, BorrowedAt = baseDate.AddDays(-45), ReturnedAt = baseDate.AddDays(-30) },
            new Loan { CopyId = copies[2].Id, UserId = users[0].Id, BorrowedAt = baseDate.AddDays(-20), ReturnedAt = baseDate.AddDays(-10) },
            new Loan { CopyId = copies[3].Id, UserId = users[2].Id, BorrowedAt = baseDate.AddDays(-14), ReturnedAt = baseDate.AddDays(-2) },
            new Loan { CopyId = copies[4].Id, UserId = users[0].Id, BorrowedAt = baseDate.AddDays(-10), ReturnedAt = null },
            new Loan { CopyId = copies[5].Id, UserId = users[3].Id, BorrowedAt = baseDate.AddDays(-5), ReturnedAt = null },
            new Loan { CopyId = copies[6].Id, UserId = users[1].Id, BorrowedAt = baseDate.AddDays(-25), ReturnedAt = baseDate.AddDays(-5) },
            new Loan { CopyId = copies[7].Id, UserId = users[2].Id, BorrowedAt = baseDate.AddDays(-18), ReturnedAt = baseDate.AddDays(-1) },
            new Loan { CopyId = copies[2].Id, UserId = users[3].Id, BorrowedAt = baseDate.AddDays(-90), ReturnedAt = baseDate.AddDays(-75) },
            new Loan { CopyId = copies[1].Id, UserId = users[2].Id, BorrowedAt = baseDate.AddDays(-5), ReturnedAt = null }
        };

        await db.Loans.AddRangeAsync(loans, ct);
        await db.SaveChangesAsync(ct);
    }
}
