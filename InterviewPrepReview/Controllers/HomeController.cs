using InterviewPrepReview.Models;
using InterviewPrepReview.Models.Home;
using InterviewPrepReview.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewPrepReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SwapiClient _swapiClient;

        public HomeController(ILogger<HomeController> logger,
            SwapiClient swapiClient)
        {
            _logger = logger;
            _swapiClient = swapiClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _swapiClient.GetAllPlanets();
            var viewModel = new IndexViewModel();
            viewModel.AllPlanets = new List<Planets>();
            var temp = new List<Planets>();

            foreach (var model in response)
            {
                temp = model.results.Select(result => new Planets()
                {
                    name = result.name,
                    climate = result.climate
                }).ToList();

                foreach (var planet in temp)
                {
                    viewModel.AllPlanets.Add(planet);
                }

            }

            return View(viewModel);
        }


        public async Task<IActionResult> Residents(ChoosePlanetViewModel model)
        {
            var response = await _swapiClient.GetOnePlanet(model.PlanetID);
            var viewModel = new ResidentsViewModel();
            viewModel.Residents = new List<People>();
            var peopleList = new List<String>();
            var residentID = "";
            var tempURL = "";


            foreach (var resident in response.residents)
            {
                tempURL = resident.Substring(0, resident.Length - 1);
                residentID = tempURL.Split('/').Last();
                peopleList.Add(residentID);
            }

            var residentResponse = await _swapiClient.GetResidents(peopleList);

            viewModel.Residents = residentResponse.Select(resident => new People()
            {
                name = resident.name,
                homeworld = resident.homeworld
            }).ToList();

            return View(viewModel);
        }

        public IActionResult ChoosePlanet()
        {
            return View();
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
    }
}
