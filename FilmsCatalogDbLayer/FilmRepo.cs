using System.Collections.Generic;
using System.Threading.Tasks;
using FilmsCatalogDbLayer.Interfaces;
using FilmsCatalogModels.Dtos;
using FilmsCatalogDbCore;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using FilmsCatalogModels.Db;
using System;

namespace FilmsCatalogDbLayer
{
    public class FilmRepo : IFilmRepo
    {
        private readonly FilmsCatalogDbContext _context;
        private readonly IMapper _mapper;

        public FilmRepo(FilmsCatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<FilmForListDto>> GetList(int pageNumber, int pageSize)
        {
            int count = 0;
            List<FilmForListDto> emptyResult = new List<FilmForListDto>();
            if (pageNumber < 0 || pageSize < 0)
                return emptyResult;

            try
            {
                count = checked(pageNumber * pageSize);
            }
            catch (OverflowException)
            {
                return emptyResult;
            }

            return await _context.Films.Include(x => x.FilmDirectors)
                        .AsNoTracking()
                        .OrderBy(f => f.Name)
                        .Skip(count)
                        .Take(pageSize)
                        .ProjectTo<FilmForListDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<FilmDto> GetById(int id)
        {
            var film = await _context.Films.Include(fd => fd.FilmDirectors)
                                           .AsNoTracking()
                                           .ProjectTo<FilmDto>(_mapper.ConfigurationProvider)
                                           .FirstOrDefaultAsync(f => f.Id == id);
            return film;
        }

        public async Task<int> InsertOrUpdate(Film film)
        {
            if (film.Id > 0)
            {
                return await UpdateFilm(film);
            }
            return await CreateFilm(film);
        }

        private async Task<int> CreateFilm(Film film)
        {
            film.CreatedDate = DateTime.Now;
            await _context.Films.AddAsync(film);
            await _context.SaveChangesAsync();

            if (film.Id == 0) throw new Exception("Невозможно создать объект");

            return film.Id;
        }

        private async Task<int> UpdateFilm(Film film)
        {
            var dbFilm = await _context.Films.Include(fd => fd.FilmDirectors).FirstOrDefaultAsync(f => f.Id == film.Id);

            dbFilm.Name = film.Name;
            dbFilm.Year = film.Year;
            dbFilm.Description = film.Description;
            dbFilm.IsActive = film.IsActive;
            
            dbFilm.Poster = film.Poster;
            dbFilm.FilmDirectors = film.FilmDirectors;
            dbFilm.LastModifiedDate = DateTime.Now;
            dbFilm.LastModifiedUserId = film.LastModifiedUserId;

            await _context.SaveChangesAsync();
            return film.Id;
        }

        public async Task Delete(int id)
        {
            var item = await _context.Films.FindAsync(id);
            if (item == null) return;
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetAllCount()
        {
            return await _context.Films.CountAsync();
        }
    }
}
