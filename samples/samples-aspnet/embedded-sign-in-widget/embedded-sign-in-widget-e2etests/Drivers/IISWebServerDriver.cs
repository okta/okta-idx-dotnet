﻿using System;
using System.Diagnostics;

namespace embedded_sign_in_widget_e2etests.Drivers
{
    public class IISWebServerDriver : IWebServerDriver
    {
        private const string WebSitePathEnvName = "EmbeddedSIWWebSitePath";

        private readonly int _iisPort;
        private readonly string _pathToWebSite;
        private Process _iisProcess;

        public string SiteUrl => $"http://localhost:{_iisPort}";
        public IISWebServerDriver()
        {
            var config = ConfigBuilder.Configuration;
            _iisPort = config.IisPort;
            _pathToWebSite = System.Environment.GetEnvironmentVariable(WebSitePathEnvName);

            if (string.IsNullOrEmpty(_pathToWebSite))
                throw new ArgumentException($"Environment variable ({WebSitePathEnvName}) is not set");
        }

        public string StartWebServer()
        {
            StartIIS();
            return SiteUrl;
        }

        public void StopWebServer()
        {
            StopIISExpress();
        }

        private void StopIISExpress()
        {
            if (_iisProcess.HasExited == false)
            {
                _iisProcess.Kill();
            }
        }

        private void StartIIS()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + "\\IIS Express\\iisexpress.exe";
            _iisProcess.StartInfo.Arguments = $"/path:\"{_pathToWebSite}\" /port:{_iisPort}";
            _iisProcess.Start();
        }

    }
}
