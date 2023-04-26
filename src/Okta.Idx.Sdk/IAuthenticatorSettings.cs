namespace Okta.Idx.Sdk
{
    public interface IAuthenticatorSettings
    {
        IAge Age { get; }
        IComplexity Complexity { get; }
        int? DaysToExpiry { get; }
    }
}