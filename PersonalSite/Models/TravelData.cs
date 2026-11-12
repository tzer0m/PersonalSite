namespace PersonalSite.Models
{
    /// <summary>
    /// The country codes listed in countries.json.
    /// </summary>
    public class TravelData
    {
        /// <summary>
        /// Codes of countries that have been visited.
        /// </summary>
        public List<string> Visited { get; set; } = [];

        /// <summary>
        /// Codes of countries on the bucket list.
        /// </summary>
        public List<string> BucketList { get; set; } = [];
    }
}