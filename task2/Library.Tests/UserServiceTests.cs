using FluentAssertions;
using Library.Application.DTOs;
using Library.Application.Services;
using Library.Tests;

namespace Library.UnitTests;

public class UserServiceTests
{
    [Fact]
    public async Task GetTopBorrowersAsync_RespectsDateRange()
    {
        using var db = TestDbFactory.CreateContext(nameof(GetTopBorrowersAsync_RespectsDateRange));
        var service = new UserService(db);

        var end = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var start = end.AddMonths(-3);

        var top = await service.GetTopBorrowersAsync(start, end, take: 5);

        top.Should().NotBeEmpty();
        top.First().UserId.Should().Be(1);
        top.First().BorrowCount.Should().Be(3);
        top.Last().BorrowCount.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(TimeGrouping.Month, 2)]
    [InlineData(TimeGrouping.Week, 3)]
    public async Task GetBorrowHistoryAsync_GroupsLoansByPeriod(TimeGrouping grouping, int expectedGroups)
    {
        using var db = TestDbFactory.CreateContext(nameof(GetBorrowHistoryAsync_GroupsLoansByPeriod) + grouping);
        var service = new UserService(db);

        var end = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var start = end.AddMonths(-3);

        var history = await service.GetBorrowHistoryAsync(1, start, end, grouping);

        history.Should().HaveCount(expectedGroups);
        history.SelectMany(h => h.Loans).Should().HaveCount(3);
        history.SelectMany(h => h.Loans).Select(l => l.BookId).Distinct().Should().Contain(2);
}
}
