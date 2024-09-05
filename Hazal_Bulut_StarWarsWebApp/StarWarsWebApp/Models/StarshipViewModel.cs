namespace StarWarsWebApp.Models
{
    public class StarshipViewModel
    {
        public List<Starship> Starships { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
        public int TotalCrew { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortOrder { get; set; }
        public int TotalCrewCurrentList { get; set; }
        public string SearchName { get; set; }
    }
}
