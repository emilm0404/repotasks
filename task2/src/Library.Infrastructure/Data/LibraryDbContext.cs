using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Copy> Copies => Set<Copy>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Loan> Loans => Set<Loan>();
    }
}
