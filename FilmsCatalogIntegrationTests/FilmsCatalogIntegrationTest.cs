using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FilmsCatalogDbCore;
using FilmsCatalogDbLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using FilmsCatalogMapperSettings;
using System.Linq;
using FilmsCatalogModels.Db;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilmsCatalogDbLayer;
using Shouldly;
using FilmsCatalogModels.Dtos;

namespace FilmsCatalogIntegrationTests
{
    public class FilmsCatalogIntegrationTest
    {
        readonly IMapper _mapper;
        readonly TestConfigurationBuilder testConfigBuilder;

        public FilmsCatalogIntegrationTest()
        {
            testConfigBuilder = new TestConfigurationBuilder();
            _mapper = TestConfigurationBuilder.Mapper;
        }

        [Theory]
        [InlineData(TestConstants.FILM1_NAME)]
        [InlineData(TestConstants.FILM2_NAME)]
        [InlineData(TestConstants.FILM3_NAME)]
        public async Task TestFilmsList(string name)
        {
            using (var context = GetContext())
            {
                var dbRepo = GetDbRepo(context);
                var films = await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE);

                films.ShouldNotBeNull();
                films.Count.ShouldBe(3);

                var film = films.FirstOrDefault(x => x.Name.Equals(name));
                film.ShouldNotBeNull();
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(int.MinValue, int.MaxValue)]
        public async Task TestFilmsListPagination(int pageNumber, int pageSize)
        {
            using (var context = GetContext())
            {
                var dbRepo = GetDbRepo(context);
                var films = await dbRepo.GetList(pageNumber, pageSize);

                films.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(TestConstants.FILM1_NAME)]
        [InlineData(TestConstants.FILM2_NAME)]
        [InlineData(TestConstants.FILM3_NAME)]
        public async Task TestGetFilmById(string name)
        {
            using (var context = GetContext())
            {
                var dbRepo = GetDbRepo(context);
                var films = await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE);

                var film = await dbRepo.GetById(films.FirstOrDefault(f => f.Name == name).Id);

                film.ShouldNotBeNull();
                film.Name.ShouldBe(name);
            }
        }

        [Fact]
        public async Task TestGetFilmByEmptyId()
        {
            using (var context = GetContext())
            {
                var dbRepo = GetDbRepo(context);
                var film = await dbRepo.GetById(-1);

                film.ShouldBeNull();
            }
        }

        [Theory]
        [InlineData(TestConstants.FILM1_NAME)]
        [InlineData(TestConstants.FILM2_NAME)]
        [InlineData(TestConstants.FILM3_NAME)]
        public async Task TestUpdateFilm(string name)
        {
            using (var context = GetContext())
            {
                var dbRepo = GetDbRepo(context);
                var films = await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE);
                var film = await dbRepo.GetById(films.FirstOrDefault(f => f.Name == name).Id);

                string modifiedName = film.Name + TestConstants.MODIFIED_SUFFIX;
                string modifiedDescription = film.Description + TestConstants.MODIFIED_SUFFIX;
                film.Name = modifiedName;
                film.Description = modifiedDescription;
                int id = await dbRepo.InsertOrUpdate(_mapper.Map<Film>(film));

                var filmChanged = await dbRepo.GetById(id);
                filmChanged.ShouldNotBeNull();
                filmChanged.Name.ShouldBe(modifiedName);
                filmChanged.Description.ShouldBe(modifiedDescription);

                await context.Database.EnsureDeletedAsync();
            }
        }

        [Theory]
        [InlineData(TestConstants.FILM1_NAME)]
        [InlineData(TestConstants.FILM2_NAME)]
        [InlineData(TestConstants.FILM3_NAME)]
        public async Task TestUpdateFilmDirectors(string name)
        {
            int id = 0;
            int film1DirsCountBegin = 0;

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);
                var director3 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR3_NAME));

                var film = context.Films.Include(i => i.FilmDirectors).Single(x => x.Name.Equals(name));
                film1DirsCountBegin = film.FilmDirectors.Count;
                film.FilmDirectors.Add(new FilmDirector
                {
                    Director = director3,
                    DirectorId = director3.Id,
                    Film = film,
                    FilmId = film.Id
                });

                id = await dbRepo.InsertOrUpdate(film);
            }

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);
                var filmChanged = await dbRepo.GetById(id);
                filmChanged.ShouldNotBeNull();
                filmChanged.Directors.Count.ShouldBe(film1DirsCountBegin + 1);
            }

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);
                var director3 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR3_NAME));

                var film = context.Films.Include(i => i.FilmDirectors).Single(x => x.Name.Equals(name));
                film1DirsCountBegin = film.FilmDirectors.Count;
                film.FilmDirectors.Remove(film.FilmDirectors.FirstOrDefault(f => f.DirectorId == director3.Id));

                id = await dbRepo.InsertOrUpdate(film);
            }

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);
                var filmChanged = await dbRepo.GetById(id);
                filmChanged.ShouldNotBeNull();
                filmChanged.Directors.Count.ShouldBe(film1DirsCountBegin - 1);

                await context.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task TestCreateFilm()
        {
            int id = 0;
            int filmsCountBegin = 0;
            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);
                var director3 = context.Directors.Single(x => x.Name.Equals(TestConstants.DIR3_NAME));

                filmsCountBegin = (await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE)).Count;

                var film = new Film()
                {
                    Name = TestConstants.FILM4_NAME,
                    Description = TestConstants.FILM4_DESC,
                    Year = 2005,
                    IsActive = true,
                    IsDeleted = false
                };
                film.FilmDirectors = new List<FilmDirector>
                {
                    new FilmDirector
                    {
                        Film = film,
                        DirectorId = director3.Id,
                        Director = director3
                    }
                };

                id = await dbRepo.InsertOrUpdate(film);
            }

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);


                var filmChanged = await dbRepo.GetById(id);
                filmChanged.ShouldNotBeNull();
                filmChanged.Directors.Count.ShouldBe(1);

                (await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE)).Count.ShouldBe(filmsCountBegin + 1);
                await context.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task TestDeleteFilm()
        {
            int id = 0;
            int filmsCountBegin = 0;
            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);

                List<FilmForListDto> films = await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE);
                filmsCountBegin = films.Count;

                id = films.FirstOrDefault(f => f.Name == TestConstants.FILM1_NAME).Id;
                await dbRepo.Delete(id);
            }

            using (var context = GetContext())
            {

                var dbRepo = GetDbRepo(context);


                var filmChanged = await dbRepo.GetById(id);
                filmChanged.ShouldBeNull();

                (await dbRepo.GetList(TestConstants.PAGE_NUMBER, TestConstants.PAGE_SIZE)).Count.ShouldBe(filmsCountBegin - 1);
                await context.Database.EnsureDeletedAsync();
            }
        }

        private IFilmRepo GetDbRepo(FilmsCatalogDbContext context)
        {
            return new FilmRepo(context, _mapper);
        }

        private FilmsCatalogDbContext GetContext()
        {
            return testConfigBuilder.GetContext();
        }

    }
}
