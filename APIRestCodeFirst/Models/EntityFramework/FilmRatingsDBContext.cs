using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APIRestCodeFirst.Models.EntityFramework
{
    public partial class FilmRatingsDBContext : DbContext
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
            /*
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLoggerFactory(MyLoggerFactory)
                                .EnableSensitiveDataLogging()
                                .UseNpgsql("Server=localhost;port=5432;Database=lesfilms;uid=postgres;password=postgres;");
            }*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utilisateur>(entity => {
                entity
                    .Property(e => e.DateCreation)
                    .HasDefaultValueSql("now()");
                entity
                    .Property(e => e.Pays)
                    .HasDefaultValue("France");
                entity
                    .HasIndex(e => e.Mail)
                    .IsUnique()
                    .HasDatabaseName("uq_utl_mail");
            });
            modelBuilder.Entity<Notation>(entity => {
                entity.HasCheckConstraint("ck_not_note", "not_note between 0 and 5");
            });
            modelBuilder.Entity<Notation>().HasKey(m => new { m.UtilisateurId, m.FilmId });

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
