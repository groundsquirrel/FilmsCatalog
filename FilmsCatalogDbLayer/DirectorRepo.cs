using AutoMapper;
using AutoMapper.QueryableExtensions;
using FilmsCatalogDbCore;
using FilmsCatalogDbLayer.Interfaces;
using FilmsCatalogModels.Db;
using FilmsCatalogModels.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalogDbLayer
{
    public class DirectorRepo : IDirectorRepo
    {
        private readonly FilmsCatalogDbContext _context;
        private readonly IMapper _mapper;

        public DirectorRepo(FilmsCatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DirectorDto>> GetList()
        {
            return await _context.Directors
                        .AsNoTracking()
                        .OrderBy(f => f.Name)
                        .ProjectTo<DirectorDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<int> InsertOrUpdate(Director director)
        {
            if (director.Id > 0)
            {
                return await Update(director);
            }
            return await Create(director);
        }

        private async Task<int> Create(Director director)
        {
            director.CreatedDate = DateTime.Now;
            await _context.Directors.AddAsync(director);
            await _context.SaveChangesAsync();

            if (director.Id == 0) throw new Exception("Невозможно создать объект");

            return director.Id;
        }

        private async Task<int> Update(Director director)
        {
            var dbFilm = await _context.Directors.FindAsync(director.Id);
            
            dbFilm.Name = director.Name;
            dbFilm.IsActive = director.IsActive;
         
            dbFilm.LastModifiedDate = DateTime.Now;
            dbFilm.LastModifiedUserId = director.LastModifiedUserId;

            await _context.SaveChangesAsync();
            return director.Id;
        }
    }
}
