using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FilmsCatalog.Models
{
    public class DirectorViewModel
    {
        private const string errorMessage = "Необходимо заполнить";
        public int Id { get; set; }
        [DisplayName("Наименование")]
        [Required(ErrorMessage = errorMessage)]
        public string Name { get; set; } = "";

        [DisplayName("Активно")]
        public bool IsActive { get; set; } = true;
    }
}