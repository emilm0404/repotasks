namespace Library.Application.DTOs
{
    public class AvailabilityDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public int TotalCopies { get; set; }
        public int Borrowed { get; set; }
        public int Available => TotalCopies - Borrowed;
    }
}
