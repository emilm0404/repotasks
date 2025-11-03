using FluentAssertions;
using Library.Application.Services;
using Library.Tests;

namespace Library.UnitTests;

public class BookServiceTests
{
    [Fact]
    public async Task GetMostBorrowedBooksAsync_ReturnsOrderedResults()
    {
        using var db = TestDbFactory.CreateContext(nameof(GetMostBorrowedBooksAsync_ReturnsOrderedResults));
        var service = new BookService(db);

        var result = await service.GetMostBorrowedBooksAsync();

        result.Should().NotBeEmpty();
        result.First().BookId.Should().Be(2);
        result.First().BorrowCount.Should().BeGreaterThan(result.Skip(1).First().BorrowCount);
    }

    [Fact]
    public async Task GetAvailabilityAsync_ReturnsBorrowedAndAvailableCounts()
    {
        using var db = TestDbFactory.CreateContext(nameof(GetAvailabilityAsync_ReturnsBorrowedAndAvailableCounts));
        var service = new BookService(db);

        var availability = await service.GetAvailabilityAsync(1);

        availability.Should().NotBeNull();
        availability!.TotalCopies.Should().Be(2);
        availability.Borrowed.Should().Be(1);
        availability.Available.Should().Be(1);
    }

    [Fact]
    public async Task GetCoBorrowedBooksAsync_ReturnsOtherBooksBorrowedBySameUsers()
    {
        using var db = TestDbFactory.CreateContext(nameof(GetCoBorrowedBooksAsync_ReturnsOtherBooksBorrowedBySameUsers));
        var service = new BookService(db);

        var alsoBorrowed = await service.GetCoBorrowedBooksAsync(1);

        alsoBorrowed.Should().NotBeEmpty();
        alsoBorrowed.First().BookId.Should().Be(2);
        alsoBorrowed.First().SharedBorrowerCount.Should().Be(2);
        alsoBorrowed.Select(b => b.BookId).Should().Contain(3);
    }

    [Fact]
    public async Task GetReadRateAsync_ComputesAveragePagesPerDay()
    {
        using var db = TestDbFactory.CreateContext(nameof(GetReadRateAsync_ComputesAveragePagesPerDay));
        var service = new BookService(db);

        var readRate = await service.GetReadRateAsync(2);

        readRate.Should().NotBeNull();
        readRate!.CompletedLoans.Should().Be(2);
        readRate.AveragePagesPerDay.Should().BeGreaterThan(0);
    }
}
