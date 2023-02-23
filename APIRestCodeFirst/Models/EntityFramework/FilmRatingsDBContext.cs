using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APIRestCodeFirst.Models.EntityFramework
{
    public class FilmRatingsDBContext : DbContext
    {
        public FilmRatingsDBContext()
        {
        }
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        public FilmRatingsDBContext(DbContextOptions<FilmRatingsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Notation> Notation { get; set; } = null!;
        public virtual DbSet<Film> Films { get; set; } = null!;
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLoggerFactory(MyLoggerFactory)
                                .EnableSensitiveDataLogging()
                                .UseNpgsql("Server=localhost;port=5432;Database=films;uid=postgres;password=postgres;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notation>()
                  .HasKey(m => new { m.UtilisateurId, m.FilmId });
            modelBuilder.Entity<Utilisateur>(entity => {
                entity.Property(e => e.DateCreation).HasDefaultValueSql("now()");
                entity.Property(e => e.Pays).HasDefaultValue("France");
            });
        }
    }
}
