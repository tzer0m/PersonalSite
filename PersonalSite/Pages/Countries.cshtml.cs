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
        /// The total number of countries and territories.
        /// </summary>
        public int Total => atlas.Countries.Count;

        /// <summary>
        /// The percentage of countries visited.
        /// </summary>
        public int Percent => Total == 0 ? 0 : (int)Math.Round(Visited.Count * 100.0 / Total);

        /// <summary>
        /// The world map SVG markup.
        /// </summary>
        public string MapSvg => atlas.MapSvg;

        /// <summary>
        /// The visited country codes as a comma separated list, for the globe script.
        /// </summary>
        public string VisitedCodes => string.Join(",", Visited.Select(country => country.Code));

        /// <summary>
        /// The CSS rule that colours the visited countries on the 2D map.
        /// </summary>
        public string MapStyles => Visited.Count == 0 ? string.Empty : $"{string.Join(", ", Visited.SelectMany(country => (string[])[$"#c-{country.Code}", $"#m-{country.Code}"]))} {{ fill: var(--color-accent); visibility: visible; }}";

        /// <summary>
        /// Handles GET requests for the countries page.
        /// </summary>
        public void OnGet()
        {
        }
    }
}