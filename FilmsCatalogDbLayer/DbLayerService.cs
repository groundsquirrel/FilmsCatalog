using FilmsCatalogDbLayer.Interfaces;

namespace FilmsCatalogDbLayer
{
    public class DbLayerService : IDbLayerService
    {
        IFilmRepo _filmRepo;
        IDirectorRepo _directorRepo;
        public DbLayerService(IFilmRepo filmRepo, IDirectorRepo directorRepo)
        {
            _filmRepo = filmRepo;
            _directorRepo = directorRepo;
        }
        public IDirectorRepo Directors => _directorRepo;

        public IFilmRepo Films => _filmRepo;
    }
}
