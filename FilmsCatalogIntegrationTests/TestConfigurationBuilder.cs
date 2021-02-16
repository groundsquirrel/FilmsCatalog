using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FilmsCatalogDbCore;
using FilmsCatalogDbLayer;
using FilmsCatalogMapperSettings;
using FilmsCatalogModels.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FilmsCatalogIntegrationTests
{
    class TestConfigurationBuilder
    {
        DbContextOptions<FilmsCatalogDbContext> _options;
        private static MapperConfiguration _mapperConfig;

        private static IServiceProvider _serviceProvider;
        private static IMapper _mapper;


        public DbContextOptions<FilmsCatalogDbContext> Options { get => _options; private set => _options = value; }
        public static IMapper Mapper { get => _mapper; private set => _mapper = value; }


        public TestConfigurationBuilder()
        {
            SetupOptions();
            BuildDefaults();
        }

        private void SetupOptions()
        {
            _options = new DbContextOptionsBuilder<FilmsCatalogDbContext>()
                            .UseInMemoryDatabase(databaseName: "FilmsCatalogTest")// + DateTime.Now.Ticks)
                            .Options;
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(FilmsCatalogMapper));
            _serviceProvider = services.BuildServiceProvider();

            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FilmsCatalogMapper>();
            });
            _mapperConfig.AssertConfigurationIsValid();
            _mapper = _mapperConfig.CreateMapper();
        }

        private void BuildDefaults()
        {
            using (var context = new FilmsCatalogDbContext(_options))
            {
                var dbRepo = new FilmRepo(context, _mapper);
                if (IsFilmsExists(context)) return;

                AddDirectors(context);
                CreateFilms(context);

                var director1 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR1_NAME));
                var director2 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR2_NAME));
                var director3 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR3_NAME));

                var film1 = context.Films.Single(x => x.Name.Equals(TestConstants.FILM1_NAME));
                var film2 = context.Films.Single(x => x.Name.Equals(TestConstants.FILM2_NAME));
                var film3 = context.Films.Single(x => x.Name.Equals(TestConstants.FILM3_NAME));

                film1.FilmDirectors = new List<FilmDirector>
                {
                    new FilmDirector
                    {
                        FilmId = film1.Id,
                        Film = film1,
                        DirectorId = director1.Id,
                        Director = director1,
                        CreatedDate = DateTime.Now
                    }
                };
                film2.FilmDirectors = new List<FilmDirector>
                {
                    new FilmDirector
                    {
                        FilmId = film2.Id,
                        Film = film2,
                        DirectorId = director1.Id,
                        Director = director1,
                        CreatedDate = DateTime.Now
                    },
                    new FilmDirector
                    {
                        FilmId = film2.Id,
                        Film = film2,
                        DirectorId = director2.Id,
                        Director = director2,
                        CreatedDate = DateTime.Now
                    }
                };
                // film3.FilmDirectors = new List<FilmDirector>
                // {
                //     new FilmDirector
                //     {
                //         FilmId = film3.Id,
                //         Film = film3,
                //         DirectorId = director3.Id,
                //         Director = director3
                //     }
                // };
                context.SaveChanges();
            }
        }

        private void CreateFilms(FilmsCatalogDbContext context)
        {
            Random rnd = new Random();
            Byte[] b = new Byte[1000];
            rnd.NextBytes(b);
            var film1 = new Film()
            {
                Name = TestConstants.FILM1_NAME,
                Description = TestConstants.FILM1_DESC,
                Year = 2020,
                IsActive = true,
                IsDeleted = false,
                Poster = b
            };
            context.Films.Add(film1);

            rnd.NextBytes(b);
            var film2 = new Film()
            {
                Name = TestConstants.FILM2_NAME,
                Description = TestConstants.FILM2_DESC,
                Year = 2015,
                IsActive = true,
                IsDeleted = false,
                Poster = b
            };
            context.Films.Add(film2);

            rnd.NextBytes(b);
            var film3 = new Film()
            {
                Name = TestConstants.FILM3_NAME,
                Description = TestConstants.FILM3_DESC,
                Year = 2010,
                IsActive = true,
                IsDeleted = false,
                Poster = b
            };
            context.Films.Add(film3);
            context.SaveChanges();
        }

        private void AddDirectors(FilmsCatalogDbContext context)
        {
            var director1 = new Director() { Name = TestConstants.DIR1_NAME, IsActive = true, CreatedDate = DateTime.Now };
            var director2 = new Director() { Name = TestConstants.DIR2_NAME, IsActive = true, CreatedDate = DateTime.Now };
            var director3 = new Director() { Name = TestConstants.DIR3_NAME, IsActive = true, CreatedDate = DateTime.Now };
            context.Directors.Add(director1);
            context.Directors.Add(director2);
            context.Directors.Add(director3);
            context.SaveChanges();
        }

        private bool IsFilmsExists(FilmsCatalogDbContext context)
        {
            var film1Detail = context.Films.SingleOrDefault(x => x.Name.Equals(TestConstants.FILM1_NAME));
            var film2Detail = context.Films.SingleOrDefault(x => x.Name.Equals(TestConstants.FILM2_NAME));
            var film3Detail = context.Films.SingleOrDefault(x => x.Name.Equals(TestConstants.FILM3_NAME));
            return (film1Detail != null || film2Detail != null || film3Detail != null);
            //return context.Films.Any();
        }

        public FilmsCatalogDbContext GetContext()
        {
            return new FilmsCatalogDbContext(_options);
        }
    }
}