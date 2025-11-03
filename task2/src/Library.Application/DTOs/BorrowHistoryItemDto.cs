namespace Library.Application.DTOs;

public class BorrowHistoryItemDto
{
    public int LoanId { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
