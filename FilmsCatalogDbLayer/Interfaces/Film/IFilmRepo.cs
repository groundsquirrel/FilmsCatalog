using FilmsCatalogModels.Db;
using FilmsCatalogModels.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmsCatalogDbLayer.Interfaces
{
    public interface IFilmRepo
    {
        Task<List<FilmForListDto>> GetList(int pageNumber, int pageSize);
        Task<FilmDto> GetById(int id);
        Task<int> GetAllCount();

        Task<int> InsertOrUpdate(Film item);
        Task Delete(int id);
    }
}
