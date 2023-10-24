namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent authenticator settings.
    /// </summary>
    public interface IAuthenticatorSettings
    {
        /// <summary>
        /// Gets the age.
        /// </summary>
        IAge Age { get; }

        /// <summary>
        /// Gets the complexity
        /// </summary>
        IComplexity Complexity { get; }

        /// <summary>
        /// Gets the days to expiry.
        /// </summary>
        int? DaysToExpiry { get; }
    }
}