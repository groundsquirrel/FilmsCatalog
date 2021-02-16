using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FilmsCatalogBusinessLayer.Interfaces;
using FilmsCatalogDbLayer.Interfaces;
using FilmsCatalogModels.Db;
using FilmsCatalogModels.Dtos;

namespace FilmsCatalogBusinessLayer
{
    public class FilmsService : IFilmsService
    {
        private readonly IDbLayerService _dbRepo;
        private readonly IMapper _mapper;

        public FilmsService(
            IDbLayerService dbRepo,
            IMapper mapper)
        {
            _dbRepo = dbRepo;
            _mapper = mapper;
        }

        public async Task DeleteFilm(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Необходим корректный ИД для удаления");
            }

            await _dbRepo.Films.Delete(id);
        }

        public async Task<int> GetAllFilmsCount()
        {
            return await _dbRepo.Films.GetAllCount();
        }

        public async Task<List<DirectorDto>> GetDirectorsList()
        {
            return await _dbRepo.Directors.GetList();
        }

        public async Task<FilmDto> GetFilmById(int id)
        {
            return await _dbRepo.Films.GetById(id);
        }

        public async Task<int> InsertOrUpdateDirector(DirectorDto item)
        {
            return await _dbRepo.Directors.InsertOrUpdate(_mapper.Map<Director>(item));
        }

        public async Task<int> InsertOrUpdateFilm(FilmDto item)
        {
            if (!item.IsEditingAllowed)
                throw new Exception("Нет доступа к редактируемому объекту");

            return await _dbRepo.Films.InsertOrUpdate(_mapper.Map<Film>(item));
        }

        public async Task<List<FilmForListDto>> ListFilms(int pageNumber, int pageSize)
        {
            return await _dbRepo.Films.GetList(pageNumber, pageSize);
        }

    }
}