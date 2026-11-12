namespace PersonalSite.Models
{
    /// <summary>
    /// A country or territory from the world reference data.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// The ISO 3166-1 alpha-2 code.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The common English name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The continent the country belongs to.
        /// </summary>
        public string Continent { get; set; } = string.Empty;
    }
}