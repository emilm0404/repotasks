namespace Library.Application.DTOs;

public class ReadRateDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public int PageCount { get; set; }
    public double AveragePagesPerDay { get; set; }
    public int CompletedLoans { get; set; }
}
