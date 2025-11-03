namespace Library.Application.DTOs
{
    public class MostBorrowedBookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int BorrowCount { get; set; }
    }
}
