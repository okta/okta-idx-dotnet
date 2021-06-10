namespace embedded_auth_with_sdk.E2ETests.Drivers
{
    public interface IWebServerDriver
    {
        string StartWebServer();

        void StopWebServer();

        string SiteUrl { get; }
    }
}
