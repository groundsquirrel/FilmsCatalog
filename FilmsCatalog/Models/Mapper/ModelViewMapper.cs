using System.Linq;
using AutoMapper;
using FilmsCatalogModels.Dtos;

namespace FilmsCatalog.Models
{
    public class ModelViewMapper : Profile
    {
        public ModelViewMapper()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {

            CreateMap<FilmDto, FilmViewModel>()
                .ForMember(x => x.AllDirectors, opt => opt.Ignore())
                .ForMember(x => x.DirectorsString, opt => opt.Ignore())
                .ForMember(x => x.Directors, opt => opt.MapFrom(y => y.Directors.Select(s => s.Id)))
                .ForMember(x => x.NewPoster, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Directors, opt => opt.Ignore());

            CreateMap<DirectorDto, DirectorViewModel>().ReverseMap();
        }
    }
}
