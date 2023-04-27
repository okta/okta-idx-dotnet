using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Complexity : Resource, IComplexity
    {
        /// <inheritdoc/>
        public int? MinLength => GetIntegerProperty("minLength");

        /// <inheritdoc/>
        public int? MinLowerCase => GetIntegerProperty("minLowerCase");

        /// <inheritdoc/>
        public int? MinUpperCase => GetIntegerProperty("minUpperCase");

        /// <inheritdoc/>
        public int? MinNumber => GetIntegerProperty("minNumber");

        /// <inheritdoc/>
        public int? MinSymbol => GetIntegerProperty("minSymbol");

        /// <inheritdoc/>
        public bool? ExcludeUserName => GetBooleanProperty("excludeUsername");

        /// <inheritdoc/>
        public IList<string> ExcludeAttributes => GetArrayProperty<string>("excludeAttributes");
    }
}
