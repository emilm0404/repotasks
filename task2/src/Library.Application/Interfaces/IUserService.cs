using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<TopBorrowerDto>> GetTopBorrowersAsync(DateTime from, DateTime to, int take = 10, CancellationToken ct = default);
    Task<IReadOnlyList<BorrowHistoryGroupDto>> GetBorrowHistoryAsync(int userId, DateTime from, DateTime to, TimeGrouping grouping, CancellationToken ct = default);
}
