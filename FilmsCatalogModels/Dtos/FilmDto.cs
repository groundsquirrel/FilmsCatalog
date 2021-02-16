using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FilmsCatalogModels.Dtos
{
    public class FilmDto
    {
        private const string errorMessage = "Необходимо заполнить";

        public int Id { get; set; }
        [DisplayName("Наименование")]
        [Required(ErrorMessage = errorMessage)]
        public string Name { get; set; } = "";
        [DisplayName("Описание")]
        public string Description { get; set; } = "";
        [DisplayName("Год")]
        [Required(ErrorMessage = errorMessage)]
        public int Year { get; set; }
        [DisplayName("Активно")]
        public bool IsActive { get; set; }

        public List<DirectorDto> Directors { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? LastModifiedUserId { get; set; }

        public byte[] Poster { get; set; }
        public bool IsEditingAllowed { get; set; } = false;

    }
}
