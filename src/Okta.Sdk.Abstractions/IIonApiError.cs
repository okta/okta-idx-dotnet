namespace Okta.Sdk.Abstractions
{
    public interface IIonApiError
    {
        /// <summary>
        /// Gets the <c>version</c> property.
        /// </summary>
        /// <value>The version.</value>
        string Version { get; }

        /// <summary>
        /// Gets the <c>errorSummary</c> property.
        /// </summary>
        /// <value>The error summary.</value>
        string ErrorSummary { get; }
    }
}