using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FilmsCatalogModels.Db
{
    public class Director : FullAuditModel
    {
        [Required]
        [StringLength(FilmsCatalogModelsConstants.MAX_NAME_LENGTH)]
        public string Name { get; set; }
    }
}
