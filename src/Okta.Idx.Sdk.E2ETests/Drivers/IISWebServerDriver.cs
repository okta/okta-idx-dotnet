﻿using System;
using System.Diagnostics;
using System.IO;

namespace Okta.Idx.Sdk.E2ETests.Drivers
{
    public class IISWebServerDriver: IWebServerDriver
    {
        private const int iisPort = 8080;
        private const string WebSitePathEnvName = "DirectAuthWebSitePath";

        private readonly string _pathToWebSite;
        private Process _iisProcess;

        public string SiteUrl => $"http://localhost:{iisPort}";
        public IISWebServerDriver()
        {
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
            _iisProcess.StartInfo.Arguments = $"/path:\"{_pathToWebSite}\" /port:{iisPort}";
            _iisProcess.Start();
        }

    }
}