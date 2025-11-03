namespace Library.Application.DTOs;

public class CoBorrowedBookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public int SharedBorrowerCount { get; set; }
    public int LoanCount { get; set; }
}
