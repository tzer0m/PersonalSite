namespace PersonalSite.Models
{
    /// <summary>
    /// Visited progress for a single continent.
    /// </summary>
    public class ContinentSummary
    {
        private const int BarWidth = 20;

        /// <summary>
        /// The continent name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The number of countries visited in the continent.
        /// </summary>
        public int Visited { get; set; }

        /// <summary>
        /// The total number of countries in the continent.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The number of filled blocks in the progress bar, with at least one shown once anything is visited.
        /// </summary>
        public int Filled => Visited == 0 ? 0 : Math.Max(1, (int)Math.Round(Visited * (double)BarWidth / Total));

        /// <summary>
        /// The filled part of the progress bar.
        /// </summary>
        public string BarFilled => new('█', Filled);

        /// <summary>
        /// The empty part of the progress bar.
        /// </summary>
        public string BarEmpty => new('░', BarWidth - Filled);
    }
}