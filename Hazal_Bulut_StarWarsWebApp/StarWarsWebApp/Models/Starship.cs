using System.ComponentModel.DataAnnotations;
namespace StarWarsWebApp.Models
{
    public class Starship
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Length { get; set; }
        public decimal LengthCount { get; set; }
        public string Crew { get; set; }
        public int CrewCount { get; set; }
        public string Passengers { get; set; }
        public string Consumables { get; set; }
        public string MGLT { get; set; }
        public string StarshipClass { get; set; }
        public List<string> Pilots { get; set; }
        public string PilotNames { get; set; }
        public List<string> Films { get; set; }
        public string FilmNames { get; set; }
    }
}
