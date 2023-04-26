using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class Age : Resource, IAge
    {
        public int? MinAgeMinutes => GetIntegerProperty("minAgeMinutes");
        public int? HistoryCount => GetIntegerProperty("historyCount");
    }
}
