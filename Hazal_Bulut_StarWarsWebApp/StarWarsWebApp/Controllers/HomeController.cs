using Microsoft.AspNetCore.Mvc;
using StarWarsWebApp.Models;
using System.Diagnostics;
using System.Linq.Expressions;

namespace StarWarsWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly StarshipService _starshipService;
        private static List<Starship>? _starshipList = new List<Starship>(); // All starships 
        private const int PageSize = 10;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public HomeController(StarshipService starshipService)
        {
            _starshipService = starshipService;
        }

        public async Task<IActionResult> Index(string sortOrder = "name", int page = 1, string searchName = "")
        {
            if (_starshipList == null || !_starshipList.Any())
            {
                _starshipList = await _starshipService.GetAllStarshipsAsync();
                _starshipList = ParseStarShips(_starshipList);
            }

            if (_starshipList == null)
                return View();

            var filteredStarships = string.IsNullOrEmpty(searchName)
             ? _starshipList
             : _starshipList.Where(s => s.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();

            // Sort the filtered list
            var sortedStarships = SortedStarShips(filteredStarships, ref sortOrder);

            // Pagination logic
            int totalItems = sortedStarships.Count();
            var paginatedStarships = sortedStarships.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            // Calculate total crew
            int totalCrew = sortedStarships.Sum(s => s.CrewCount);
            int totalCrewCurrentList = paginatedStarships.Sum(s => s.CrewCount);

            // Prepare ViewModel
            var viewModel = new StarshipViewModel
            {
                Starships = paginatedStarships,
                TotalCrew = totalCrew,
                TotalCrewCurrentList = totalCrewCurrentList,
                SortOrder = sortOrder,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / PageSize),
                SearchName = searchName
            };
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(viewModel);
        }

        public List<Starship> SortedStarShips(List<Starship> starShips, ref string sortOrder)
        {
            if (starShips == null || !starShips.Any())
                return new List<Starship>();

            if (sortOrder == null)
                sortOrder = "name";

            // Define sorting parameters
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.ModelSortParm = sortOrder == "model" ? "model_desc" : "model";
            ViewBag.ManufacturerSortParm = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewBag.LengthSortParm = sortOrder == "length" ? "length_desc" : "length";
            ViewBag.CrewSortParm = sortOrder == "crew" ? "crew_desc" : "crew";
            ViewBag.PassengersSortParm = sortOrder == "passengers" ? "passengers_desc" : "passengers";
            ViewBag.ConsumablesSortParm = sortOrder == "consumables" ? "consumables_desc" : "consumables";
            ViewBag.MGLTSortParm = sortOrder == "mglt" ? "mglt_desc" : "mglt";
            ViewBag.StarshipClassSortParm = sortOrder == "starshipClass" ? "starshipClass_desc" : "starshipClass";
            ViewBag.FilmsSortParm = sortOrder == "films" ? "films_desc" : "films";
            ViewBag.PilotsSortParm = sortOrder == "pilots" ? "pilots_desc" : "pilots";

            // Dictionary to map sort order to the respective property name
            var sortExpressions = new Dictionary<string, Expression<Func<Starship, object>>>
            {
                { "name", s => s.Name },
                { "name_desc", s => s.Name },
                { "model", s => s.Model },
                { "model_desc", s => s.Model },
                { "manufacturer", s => s.Manufacturer },
                { "manufacturer_desc", s => s.Manufacturer },
                { "length", s => s.LengthCount },
                { "length_desc", s => s.LengthCount },
                { "crew", s => s.CrewCount },
                { "crew_desc", s => s.CrewCount },
                { "passengers", s => s.Passengers },
                { "passengers_desc", s => s.Passengers },
                { "consumables", s => s.Consumables },
                { "consumables_desc", s => s.Consumables },
                { "mglt", s => s.MGLT },
                { "mglt_desc", s => s.MGLT },
                { "starshipClass", s => s.StarshipClass },
                { "starshipClass_desc", s => s.StarshipClass },
                { "films", s => s.FilmNames },
                { "films_desc", s => s.FilmNames },
                { "pilots", s => s.PilotNames },
                { "pilots_desc", s => s.PilotNames }
            };

            // Default sort expression (by Name ascending)
            var sortExpression = sortExpressions.ContainsKey(sortOrder) ? sortExpressions[sortOrder] : sortExpressions["name"];

            // Convert Expression<Func<T, object>> to Func<T, object>
            Func<Starship, object> func = sortExpression.Compile();

            // Apply sorting
            return sortOrder.EndsWith("_desc")
                ? starShips.OrderByDescending(func).ToList()
                : starShips.OrderBy(func).ToList();
        }

        [HttpPost]
        public IActionResult Create(Starship starship)
        {
            if (starship != null)
            {
                _starshipList.Add(starship);
                TempData["SuccessMessage"] = "Starship successfully created!";
            }
            return RedirectToAction("Index");
        }

        private List<Starship> ParseStarShips(List<Starship> starshipsList)
        {
            if (starshipsList.Count == 0)
                return starshipsList;

            return starshipsList.Select(starship =>
            {
                starship.CrewCount = ParseCrew(starship.Crew);
                starship.LengthCount = ParseLength(starship.Length);

                return starship;
            }).ToList();
        }

        private int ParseCrew(string crew)
        {
            if (string.IsNullOrWhiteSpace(crew))
                return 0;

            crew = crew.Replace("-", "").Replace(",", "").Trim();

            if (int.TryParse(crew, out int crewCount))
            {
                return crewCount;
            }

            return 0;
        }

        private decimal ParseLength(string length)
        {
            if (string.IsNullOrWhiteSpace(length))
                return 0;

           if (decimal.TryParse(length, out decimal lengthCount))
            {
                return lengthCount;
            }

            return 0;
        }
    }
}

