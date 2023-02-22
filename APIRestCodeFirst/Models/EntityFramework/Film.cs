using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;

namespace APIRestCodeFirst.Models.EntityFramework
{
    [Table("t_e_film_flm")]
    public class Film
    {
        public Film()
        { }

        [Key]
        [Column("flm_id")]
        public int FilmId { get; set; }

        [Required]
        [Column("flm_titre")]
        [StringLength(100)]
        public string Titre { get; set; }

        [Column("flm_resume")]
        public string Resume { get; set; }

        [Column("flm_datesortie")]
        public DateTime DateSortie { get; set; }

        [Column("flm_duree")]
        public Decimal Duree { get; set; }

        [Column("flm_genre")]
        [StringLength(50)]
        public string Genre { get; set; }

        [InverseProperty("NotesFilm")]
        public virtual ICollection<Notation> NotesNavigation { get; set; }
    }
}
