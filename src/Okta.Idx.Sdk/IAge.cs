namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent age.
    /// </summary>
    public interface IAge
    {
        /// <summary>
        /// Gets the history count.
        /// </summary>
        int? HistoryCount { get; }

        /// <summary>
        /// Gets the minimun age in minutes.
        /// </summary>
        int? MinAgeMinutes { get; }
    }
}