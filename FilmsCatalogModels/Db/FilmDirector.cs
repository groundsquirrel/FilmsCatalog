using System.ComponentModel.DataAnnotations;

namespace FilmsCatalogModels.Db
{
    public class FilmDirector : FullAuditModel
    {
        [Required]
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
        [Required]
        public int DirectorId { get; set; }
        public virtual Director Director { get; set; }
    }
}
