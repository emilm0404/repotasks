using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests;

internal static class TestDbFactory
{
    public static LibraryDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new LibraryDbContext(options);
        Seed(context);
        return context;
    }

    private static void Seed(LibraryDbContext db)
    {
        if (db.Books.Any())
        {
            return;
        }

        var baseDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Author = "Author One", PageCount = 400 },
            new Book { Id = 2, Title = "Book B", Author = "Author Two", PageCount = 600 },
            new Book { Id = 3, Title = "Book C", Author = "Author Three", PageCount = 300 }
        };

        var copies = new[]
        {
            new Copy { Id = 1, BookId = 1 },
            new Copy { Id = 2, BookId = 1 },
            new Copy { Id = 3, BookId = 2 },
            new Copy { Id = 4, BookId = 2 },
            new Copy { Id = 5, BookId = 3 },
            new Copy { Id = 6, BookId = 2 }
        };

        var users = new[]
        {
            new User { Id = 1, FullName = "Alice Johnson" },
            new User { Id = 2, FullName = "Bob Smith" },
            new User { Id = 3, FullName = "Charlie Nguyen" }
        };

        var loans = new[]
        {
            new Loan { Id = 1, CopyId = 1, UserId = 1, BorrowedAt = baseDate.AddDays(-30), ReturnedAt = baseDate.AddDays(-25) },
            new Loan { Id = 2, CopyId = 2, UserId = 2, BorrowedAt = baseDate.AddDays(-10), ReturnedAt = null },
            new Loan { Id = 3, CopyId = 3, UserId = 1, BorrowedAt = baseDate.AddDays(-40), ReturnedAt = baseDate.AddDays(-34) },
            new Loan { Id = 4, CopyId = 4, UserId = 2, BorrowedAt = baseDate.AddDays(-20), ReturnedAt = baseDate.AddDays(-16) },
            new Loan { Id = 5, CopyId = 6, UserId = 3, BorrowedAt = baseDate.AddDays(-5), ReturnedAt = null },
            new Loan { Id = 6, CopyId = 5, UserId = 1, BorrowedAt = baseDate.AddDays(-15), ReturnedAt = baseDate.AddDays(-5) }
        };

        db.Books.AddRange(books);
        db.Copies.AddRange(copies);
        db.Users.AddRange(users);
        db.Loans.AddRange(loans);
        db.SaveChanges();
    }
}
