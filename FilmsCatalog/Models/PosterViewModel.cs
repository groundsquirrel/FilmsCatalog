using Microsoft.AspNetCore.Http;
namespace FilmsCatalog.Models
{
    public class PosterViewModel
    {
        public string Name { get; set; }
        public IFormFile Avatar { get; set; }
    }
}