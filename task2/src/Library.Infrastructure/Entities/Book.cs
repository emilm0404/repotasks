namespace Library.Infrastructure.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int PageCount { get; set; }
        public ICollection<Copy> Copies { get; set; } = new List<Copy>();
    }
}
