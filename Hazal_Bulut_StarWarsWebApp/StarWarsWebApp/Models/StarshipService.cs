using Newtonsoft.Json;

namespace StarWarsWebApp.Models
{
    public class StarshipService
    {
        private readonly HttpClient _httpClient;

        public StarshipService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StarshipResponse?> GetStarshipsAsync(string url)
        {
            if (url == null)
                url = "https://swapi.dev/api/starships/";

            var response = await _httpClient.GetStringAsync(url);
            var starshipResponse = JsonConvert.DeserializeObject<StarshipResponse>(response);

            return starshipResponse;
        }

        public async Task<List<Starship>> GetAllStarshipsAsync()
        {
            var allStarships = new List<Starship>();
            var url = "https://swapi.dev/api/starships/";
            StarshipResponse starshipResponse = null;

            for (int i = 0; !string.IsNullOrEmpty(url); i++)
            {
                starshipResponse = await GetStarshipsAsync(url);

                if (starshipResponse != null)
                {
                    allStarships.AddRange(starshipResponse.Results);
                    url = starshipResponse.Next;
                }
            }

            foreach (var starship in allStarships)
            {
                if (starship.Films != null && starship.Films.Count > 0)
                {
                    starship.FilmNames = await GetNamesFromUrlAsync(starship.Films, false);
                }
            }
            foreach (var starship in allStarships)
            {
                if (starship.Pilots != null && starship.Pilots.Count > 0)
                {
                    starship.PilotNames = await GetNamesFromUrlAsync(starship.Pilots, true);
                }
            }
            return allStarships;
        }

        private async Task<string> GetNamesFromUrlAsync(List<string> urls, bool isPilot)
        {
            var nameList = new List<string>();

            foreach (var url in urls)
            {
                var response = await _httpClient.GetStringAsync(url);

                if (isPilot)
                {
                    var pilot = JsonConvert.DeserializeObject<Pilot>(response);
                    if (pilot != null && pilot.Name.Length > 0)
                        nameList.Add(pilot.Name);
                }
                else
                {
                    var film = JsonConvert.DeserializeObject<Film>(response);
                    if (film != null && film.Title.Length > 0)
                        nameList.Add(film.Title);
                }
            }
            return string.Join(", ", nameList);
        }
    }
}
