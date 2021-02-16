using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FilmsCatalogModels.Dtos
{
    public class FilmForListDto
    {
        public int Id { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; } = "";
        [DisplayName("Описание")]
        public string Description { get; set; } = "";
        [DisplayName("Год выпуска")]
        public int Year { get; set; } = DateTime.Now.Year;
        public bool IsActive { get; set; } = true;
        [DisplayName("Режиссёр")]
        public string Directors { get; set; } = "";
        public Guid? CreatedByUserId { get; set; }
        public bool IsEditingAllowed { get; set; } = false;

    }
}
