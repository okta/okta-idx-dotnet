using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class Complexity : Resource, IComplexity
    {
        public int? MinLength => GetIntegerProperty("minLength");

        public int? MinLowerCase => GetIntegerProperty("minLowerCase");

        public int? MinUpperCase => GetIntegerProperty("minUpperCase");

        public int? MinNumber => GetIntegerProperty("minNumber");

        public int? MinSymbol => GetIntegerProperty("minSymbol");

        public bool? ExcludeUserName => GetBooleanProperty("excludeUsername");

        public IList<string> ExcludeAttributes => GetArrayProperty<string>("excludeAttributes");
    }
}
