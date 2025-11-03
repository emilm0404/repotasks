namespace Library.Infrastructure.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public int CopyId { get; set; }
        public Copy Copy { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedAt { get; set; }
    }
}
