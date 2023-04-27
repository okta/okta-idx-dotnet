using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Age : Resource, IAge
    {
        /// <inheritdoc/>
        public int? MinAgeMinutes => GetIntegerProperty("minAgeMinutes");

        /// <inheritdoc/>
        public int? HistoryCount => GetIntegerProperty("historyCount");
    }
}
