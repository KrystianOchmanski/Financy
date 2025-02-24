using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries" },
                new Category { Id = 2, Name = "House" },
                new Category { Id = 3, Name = "Food" },
                new Category { Id = 4, Name = "Transport" },
                new Category { Id = 5, Name = "Entertainment" },
                new Category { Id = 6, Name = "Personal" },
                new Category { Id = 7, Name = "Health care" },
                new Category { Id = 8, Name = "Education" },
                new Category { Id = 9, Name = "Travel" },
                new Category { Id = 10, Name = "Finances" },
                new Category { Id = 11, Name = "Others" }
            );
        }
    }
}
