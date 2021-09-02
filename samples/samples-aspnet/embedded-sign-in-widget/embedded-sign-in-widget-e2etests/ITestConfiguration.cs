namespace embedded_sign_in_widget_e2etests
{
    public interface ITestConfiguration
    {
        public string OktaUserEmail { get; set; }
        public string OktaUserPassword { get; set; }
        public string OktaSocialIdpMfaUserEmail { get; set; }
        public string OktaSocialIdpMfaUserPassword { get; set; }
        int IisPort { get; set; }
        string SiteUrl { get; set; }
        string ScreenshotsFolder { get; set; }
    }
}
