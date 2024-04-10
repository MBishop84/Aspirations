using Aspirations.ApiService.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspirations.ApiService.Storage
{
    public class ApiContext : DbContext
    {
        public DbSet<Quote> Quotes { get; init; }
        public DbSet<Author> Authors { get; init; }
        public DbSet<JsTransform> JsTransforms { get; init; }

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
                e.HasKey(k => k.Id).HasAnnotation("SqlServer:Identity", "1, 1");
                e.Property(p => p.Name);
                e.HasMany(p => p.Quotes)
                    .WithOne()
                    .HasForeignKey(fk => fk.AuthorId)
                    .HasPrincipalKey(pk => pk.Id);
            });

            modelBuilder.Entity<Quote>(e =>
            {
                e.HasKey(k => k.Id).HasAnnotation("SqlServer:Identity", "1, 1");
                e.Property(p => p.Text);
                e.HasOne(p => p.Author)
                    .WithMany()
                    .HasForeignKey(fk => fk.AuthorId)
                    .HasPrincipalKey(pk => pk.Id);
            });

            modelBuilder.Entity<JsTransform>(e =>
            {
                e.HasKey(k => k.Id).HasAnnotation("SqlServer:Identity", "1, 1");
                e.Property(p => p.AddedBy);
                e.Property(p => p.AddedOn);
                e.Property(p => p.Name);
                e.Property(p => p.Code);
            });
        }
    }
}
