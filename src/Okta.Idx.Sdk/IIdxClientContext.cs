namespace Okta.Idx.Sdk
{
    public interface IIdxClientContext
    {
        string CodeVerifier { get; }

        string CodeChallenge { get; }

        string CodeChallengeMethod { get; }
    }
}