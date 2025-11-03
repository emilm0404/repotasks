namespace Library.Application.DTOs;

public class BorrowHistoryGroupDto
{
    public string Period { get; set; } = "";
    public IReadOnlyList<BorrowHistoryItemDto> Loans { get; set; } = Array.Empty<BorrowHistoryItemDto>();
}
