namespace Library.Infrastructure.Entities
{
    public class Copy
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
