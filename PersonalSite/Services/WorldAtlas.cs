using System.Text.Json;
using PersonalSite.Models;

namespace PersonalSite.Services
{
    /// <summary>
    /// Loads the world reference data, map and visited list from the Data folder.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public class WorldAtlas(ILogger<WorldAtlas> logger)
    {
        private static readonly string DataPath = Path.Combine(AppContext.BaseDirectory, "Data");

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly TravelData travel = Load<TravelData>("countries.json") ?? new TravelData();

        /// <summary>
        /// All countries and territories, ordered by name.
        /// </summary>
        public List<Country> Countries { get; } = Load<List<Country>>("world.json") ?? [];

        /// <summary>
        /// The world map SVG markup.
        /// </summary>
        public string MapSvg { get; } = File.ReadAllText(Path.Combine(DataPath, "world.svg"));

        /// <summary>
        /// The visited countries, ordered by name.
        /// </summary>
        public List<Country> Visited => field ??= Resolve(travel.Visited);

        /// <summary>
        /// Deserialises a JSON file from the Data folder.
        /// </summary>
        /// <typeparam name="T">The type to deserialise to.</typeparam>
        /// <param name="fileName">The file name within the Data folder.</param>
        private static T? Load<T>(string fileName) => JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(DataPath, fileName)), JsonOptions);

        /// <summary>
        /// Maps country codes to countries, logging any codes that aren't recognised.
        /// </summary>
        /// <param name="codes">The country codes.</param>
        private List<Country> Resolve(List<string> codes)
        {
            List<string> unknown = [.. codes.Where(code => !Countries.Any(country => country.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))];
            if (unknown.Count > 0)
                logger.LogWarning("Unknown country codes in countries.json: {Codes}", string.Join(", ", unknown));
            return [.. Countries.Where(country => codes.Contains(country.Code, StringComparer.OrdinalIgnoreCase))];
        }
    }
}