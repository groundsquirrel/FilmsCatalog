using System.Collections.Generic;
using FilmsCatalogModels.Dtos;

namespace FilmsCatalog.Models
{
    public class FilmsViewModel
    {
        public List<FilmForListDto> Films { get; set; }
        public bool IsAuthenticated { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int AllCount { get; set; }
    }
}