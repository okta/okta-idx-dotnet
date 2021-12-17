using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class RemediationOptionParameter : Resource
    {
        public string Label => GetStringProperty("label");

        public string Value => GetStringProperty("value");
    }
}
