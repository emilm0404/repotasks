using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBookService
{
    Task<IReadOnlyList<MostBorrowedBookDto>> GetMostBorrowedBooksAsync(int take = 10, CancellationToken ct = default);
    Task<AvailabilityDto?> GetAvailabilityAsync(int bookId, CancellationToken ct = default);
    Task<IReadOnlyList<CoBorrowedBookDto>> GetCoBorrowedBooksAsync(int bookId, CancellationToken ct = default);
    Task<ReadRateDto?> GetReadRateAsync(int bookId, CancellationToken ct = default);
}
