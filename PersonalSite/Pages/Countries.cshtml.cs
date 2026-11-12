using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalSite.Models;
using PersonalSite.Services;

namespace PersonalSite.Pages
{
    /// <summary>
    /// Page model for the countries visited map.
    /// </summary>
    /// <param name="atlas">The world atlas.</param>
    public class CountriesModel(WorldAtlas atlas) : PageModel
    {
        /// <summary>
        /// The visited countries.
        /// </summary>
        public List<Country> Visited => atlas.Visited;

        /// <summary>
        /// The bucket list countries.
        /// </summary>
        public List<Country> BucketList => atlas.BucketList;

        /// <summary>
        /// The total number of countries and territories.
        /// </summary>
        public int Total => atlas.Countries.Count;

        /// <summary>
        /// The percentage of countries visited.
        /// </summary>
        public int Percent => Total == 0 ? 0 : (int)Math.Round(Visited.Count * 100.0 / Total);

        /// <summary>
        /// Visited progress per continent.
        /// </summary>
        public List<ContinentSummary> Continents => [.. atlas.Countries.GroupBy(country => country.Continent).OrderBy(group => group.Key).Select(group => new ContinentSummary { Name = group.Key, Visited = group.Count(Visited.Contains), Total = group.Count() })];

        /// <summary>
        /// The world map SVG markup.
        /// </summary>
        public string MapSvg => atlas.MapSvg;

        /// <summary>
        /// CSS rules that colour the visited and bucket list countries on the map.
        /// </summary>
        public string MapStyles => $"{Rule(BucketList, "--color-accent")}\n{Rule(Visited, "--color-status")}";

        /// <summary>
        /// Handles GET requests for the countries page.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Builds a CSS rule that fills the given countries and shows their markers.
        /// </summary>
        /// <param name="countries">The countries to colour.</param>
        /// <param name="colour">The CSS variable holding the fill colour.</param>
        private static string Rule(List<Country> countries, string colour) => countries.Count == 0 ? string.Empty : $"{string.Join(", ", countries.SelectMany(country => (string[])[$"#c-{country.Code}", $"#m-{country.Code}"]))} {{ fill: var({colour}); visibility: visible; }}";
    }
}