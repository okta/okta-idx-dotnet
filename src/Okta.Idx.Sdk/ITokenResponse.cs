using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface ITokenResponse : IResource
    {
        /// <summary>
        /// Gets the token type.
        /// </summary>
        string TokenType { get; }

        /// <summary>
        /// Gets the expires in property.
        /// </summary>
        int? ExpiresIn { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the ID token.
        /// </summary>
        string IdToken { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        string Scope { get; }
    }
}