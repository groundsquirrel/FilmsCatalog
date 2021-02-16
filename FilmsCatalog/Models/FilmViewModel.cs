using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FilmsCatalogModels.Dtos;
using Microsoft.AspNetCore.Http;

namespace FilmsCatalog.Models
{
    public class FilmViewModel
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
        public int Year { get; set; } = DateTime.Now.Year;
        [DisplayName("Активно")]
        public bool IsActive { get; set; } = true;

        public List<DirectorDto> AllDirectors { get; set; }
        public string[] Directors { get; set; } = new string[0];
        public Guid? CreatedByUserId { get; set; }
        public IFormFile NewPoster { get; set; }
        public byte[] Poster { get; set; }
        [DisplayName("Режиссёр")]
        public string DirectorsString { get; set; }
        public bool IsEditingAllowed { get; set; }
    }
}