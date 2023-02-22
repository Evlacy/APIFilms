using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIRestCodeFirst.Models.EntityFramework
{
    [Table("t_j_notation_not")]
    public class Notation
    {
        public Notation()
        { }

        [Key]
        [Column("utl_id")]
        public int UtilisateurId { get; set; }

        [Key]
        [Column("flm_id")]
        [StringLength(100)]
        public int FilmId { get; set; }

        [Required]
        [Column("not_note")]
        [Range(0, 5)]
        public int Note { get; set; }

        [InverseProperty("UtilisateurNotant")]
        public virtual ICollection<Utilisateur> UtilisateurNavigation { get; set; }

        [InverseProperty("FilmNote")]
        public virtual ICollection<Film> FilmNavigation { get; set; }

    }
}
