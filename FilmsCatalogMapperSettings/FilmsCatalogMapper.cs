using System.Linq;
using AutoMapper;
using FilmsCatalogModels.Db;
using FilmsCatalogModels.Dtos;

namespace FilmsCatalogMapperSettings
{
    public class FilmsCatalogMapper : Profile
    {
        public FilmsCatalogMapper()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {
            CreateMap<Director, DirectorDto>().ReverseMap();
            CreateMap<Film, FilmDto>()
                .ForMember(x => x.Directors, opt => opt.MapFrom(y => y.FilmDirectors.Select(s => s.Director)))
                .ForMember(x => x.IsEditingAllowed, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.FilmDirectors, opt => opt.MapFrom(y => y.Directors.Select(s => new FilmDirector
                {
                    DirectorId = s.Id,
                    FilmId = y.Id
                })));

            CreateMap<Film, FilmForListDto>()
                .ForMember(x => x.Directors, opt => opt.MapFrom(y => string.Join(", ", y.FilmDirectors.Select(s => s.Director.Name))))
                .ForMember(x => x.IsEditingAllowed, opt => opt.Ignore());
        }
    }
}
