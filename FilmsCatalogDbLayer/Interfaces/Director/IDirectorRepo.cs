using FilmsCatalogModels.Db;
using FilmsCatalogModels.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmsCatalogDbLayer.Interfaces
{
    public interface IDirectorRepo
    {
        Task<List<DirectorDto>> GetList();
        Task<int> InsertOrUpdate(Director item);
    }
}
