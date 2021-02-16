using FilmsCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FilmsCatalogBusinessLayer.Interfaces;
using FilmsCatalogModels.Dtos;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using FilmsCatalog.Helpers;

namespace FilmsCatalog.Controllers
{
    public class DirectorController : Controller
    {
        private readonly ILogger<DirectorController> _logger;
        private readonly IFilmsService _filmsService;
        private readonly IMapper _mapper;

        public DirectorController(ILogger<DirectorController> logger, IFilmsService filmsService, IMapper mapper)
        {
            _logger = logger;
            _filmsService = filmsService;
            _mapper = mapper;
        }

        [Authorize]
        public IActionResult Create()
        {
            return View(new DirectorViewModel
            {
                IsActive = true
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DirectorViewModel item)
        {

            var dto = _mapper.Map<DirectorDto>(item);
            dto.CreatedByUserId = CommonHelper.GetUserId(User);

            if (ModelState.IsValid)
            {
                await _filmsService.InsertOrUpdateDirector(dto);

                return RedirectToAction(nameof(Index), "Home");
            }

            return View(item);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
