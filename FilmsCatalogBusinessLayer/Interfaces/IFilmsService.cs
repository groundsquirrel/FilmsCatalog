using FilmsCatalogModels.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmsCatalogBusinessLayer.Interfaces
{
    public interface IFilmsService
    {
        Task<List<FilmForListDto>> ListFilms(int pageNumber, int pageSize);
        Task<FilmDto> GetFilmById(int id);
        Task<int> GetAllFilmsCount();
        Task<List<DirectorDto>> GetDirectorsList();

        Task<int> InsertOrUpdateFilm(FilmDto item);
        Task DeleteFilm(int id);
        Task<int> InsertOrUpdateDirector(DirectorDto item);
    }
}
