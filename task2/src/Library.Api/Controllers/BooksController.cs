using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService) => _bookService = bookService;

    [HttpGet("most-borrowed")]
    [ProducesResponseType(typeof(IReadOnlyList<MostBorrowedBookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMostBorrowed([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var result = await _bookService.GetMostBorrowedBooksAsync(take, ct);
        return Ok(result);
    }

    [HttpGet("{bookId:int}/availability")]
    [ProducesResponseType(typeof(AvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailability(int bookId, CancellationToken ct)
    {
        var availability = await _bookService.GetAvailabilityAsync(bookId, ct);
        return availability is null ? NotFound() : Ok(availability);
    }

    [HttpGet("{bookId:int}/also-borrowed")]
    [ProducesResponseType(typeof(IReadOnlyList<CoBorrowedBookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlsoBorrowed(int bookId, CancellationToken ct)
    {
        var result = await _bookService.GetCoBorrowedBooksAsync(bookId, ct);
        return Ok(result);
    }

    [HttpGet("{bookId:int}/read-rate")]
    [ProducesResponseType(typeof(ReadRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReadRate(int bookId, CancellationToken ct)
    {
        var readRate = await _bookService.GetReadRateAsync(bookId, ct);
        return readRate is null ? NotFound() : Ok(readRate);
    }
}
