using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class Idp : Resource, IIdp
    {
        public string Id => GetStringProperty("id");

        public string Name => GetStringProperty("name");
    }
}
