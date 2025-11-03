namespace Library.Application.DTOs
{
    public class TopBorrowerDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int BorrowCount { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
