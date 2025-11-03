namespace Library.Infrastructure.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
