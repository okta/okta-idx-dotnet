namespace Okta.Idx.Sdk.E2ETests.Drivers
{
    public interface IWebServerDriver
    {
        string StartWebServer();

        void StopWebServer();

        string SiteUrl { get; }
    }
}
