﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests
{
    public class TestConfig : ITestConfig
    {
        public string NormalUser { get; set; }
        public string DeactivatedUser { get; set; }
        public string LockedUser { get; set; }
        public string SuspendedUser { get; set; }
        public string UnassignedUser { get; set; }
        public string UserPassword { get; set; }
        public string A18nApiKey { get; set; }
        public string A18nProfileId { get; set; }
        public int IisPort { get; set; }
    }
}
