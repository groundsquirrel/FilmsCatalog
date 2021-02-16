using FilmsCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FilmsCatalogBusinessLayer.Interfaces;
using FilmsCatalogModels;
using FilmsCatalogModels.Dtos;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using FilmsCatalog.Helpers;

namespace FilmsCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFilmsService _filmsService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IFilmsService filmsService, IMapper mapper)
        {
            _logger = logger;
            _filmsService = filmsService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = FilmsCatalogModelsConstants.PAGE_NUMBER,
                                               int pageSize = FilmsCatalogModelsConstants.PAGE_SIZE)
        {
            var films = await _filmsService.ListFilms(pageNumber, pageSize);
            foreach (var film in films)
            {
                film.IsEditingAllowed = GetIsEditingAllowed(film.CreatedByUserId);
            }

            FilmsViewModel model = new FilmsViewModel
            {
                Films = films,
                IsAuthenticated = User.Identity.IsAuthenticated,
                PageNumber = pageNumber,
                PageSize = pageSize,
                AllCount = await _filmsService.GetAllFilmsCount()
            };
            return base.View(model);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View(new FilmViewModel
            {
                Year = DateTime.Now.Year,
                IsActive = true,
                IsEditingAllowed = true,
                AllDirectors = await _filmsService.GetDirectorsList()
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmViewModel item)
        {
            var dto = _mapper.Map<FilmDto>(item);
            dto.Directors = await FilterDirectorsByIds(item.Directors);
            dto.IsEditingAllowed = true;
            dto.CreatedByUserId = CommonHelper.GetUserId(User);

            dto.Poster = item.NewPoster?.Length > 0 ? CommonHelper.ConvertFileToBinary(item.NewPoster) : null;

            if (ModelState.IsValid)
            {
                await _filmsService.InsertOrUpdateFilm(dto);

                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            FilmViewModel filmviewModel = await PrepareView(id);
            return View(filmviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmViewModel item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var filmDto = await _filmsService.GetFilmById(id);
                    if (!GetIsEditingAllowed(filmDto.CreatedByUserId))
                        return NotFound();

                    var dto = _mapper.Map<FilmDto>(item);
                    dto.CreatedByUserId = filmDto.CreatedByUserId;
                    dto.LastModifiedUserId = CommonHelper.GetUserId(User);
                    dto.IsEditingAllowed = true;
                    dto.Directors = await FilterDirectorsByIds(item.Directors);
                    dto.Poster = item.NewPoster?.Length > 0 ? CommonHelper.ConvertFileToBinary(item.NewPoster) : filmDto.Poster;

                    await _filmsService.InsertOrUpdateFilm(dto);
                }
                catch
                {
                    return BadRequest();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(await PrepareView(id));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            FilmViewModel filmviewModel = await PrepareView(id);
            return View(filmviewModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            FilmViewModel filmviewModel = await PrepareView(id);
            return View(filmviewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            try
            {
                await _filmsService.DeleteFilm(id);
            }
            catch
            {
                BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Вспомогательные методы

        /// <summary>
        /// Проверка доступа для редактирования файла
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        private bool GetIsEditingAllowed(Guid? userId)
        {
            Guid currentUserId = CommonHelper.GetUserId(User);
            return currentUserId != Guid.Empty
                   && userId.HasValue
                   && userId != Guid.Empty
                   && currentUserId == userId.Value;
        }

        /// <summary>
        /// Подготовить представление
        /// </summary>
        private async Task<FilmViewModel> PrepareView(int id)
        {
            var filmDto = await _filmsService.GetFilmById(id);

            var filmviewModel = _mapper.Map<FilmViewModel>(filmDto);
            filmviewModel.IsEditingAllowed = GetIsEditingAllowed(filmDto.CreatedByUserId);
            filmviewModel.AllDirectors = await _filmsService.GetDirectorsList();
            filmviewModel.DirectorsString = string.Join(", ", (await FilterDirectorsByIds(filmviewModel.Directors)).Select(s => s.Name));
            return filmviewModel;
        }

        /// <summary>
        /// Отфильтровать режиссёров по массиву ИД
        /// </summary>
        private async Task<List<DirectorDto>> FilterDirectorsByIds(string[] idArray)
        {
            var allDirectors = (await _filmsService.GetDirectorsList());
            int i = 0;
            var dirIds = Array.ConvertAll(idArray, s => int.TryParse(s, out i) ? i : 0);
            List<DirectorDto> directorDtos = allDirectors.Where(w => dirIds.Contains(w.Id)).ToList();
            return directorDtos;
        }

        #endregion
    }
}
