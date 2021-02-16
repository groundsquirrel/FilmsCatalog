using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FilmsCatalogModels.Db
{
    public class Film : FullAuditModel
    {
        [StringLength(FilmsCatalogModelsConstants.MAX_NAME_LENGTH)]
        [Required]
        public string Name { get; set; }

        [Range(FilmsCatalogModelsConstants.MINIMUM_YEAR, FilmsCatalogModelsConstants.MAXIMUM_YEAR)]
        public int Year { get; set; }

        [StringLength(FilmsCatalogModelsConstants.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }

        public byte[] Poster { get; set; }

        public virtual List<FilmDirector> FilmDirectors { get; set; } = new List<FilmDirector>();


    }
}
