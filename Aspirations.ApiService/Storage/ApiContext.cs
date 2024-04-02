using Aspirations.ApiService.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspirations.ApiService.Storage
{
    public class ApiContext : DbContext
    {
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring
            (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "ApiDatabase");
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(p => p.Name);
            });

            modelBuilder.Entity<Quote>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(p => p.Text);
                e.HasOne(p => p.Author)
                    .WithMany()
                    .HasForeignKey(fk => fk.Id);
            });
        }
    }
}
