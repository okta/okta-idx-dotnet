using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.IdentityEngine.Sdk
{
    public class FormValue : Resource, IFormValue
    {
        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Label => GetStringProperty("label");

        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        // TODO: Potential Resource
        public string Value => GetStringProperty("value");

        /// <inheritdoc/>
        public bool? Visible => GetBooleanProperty("visible");

        /// <inheritdoc/>
        public bool? Mutable => GetBooleanProperty("mutable");

        /// <inheritdoc/>
        public bool? Required => GetBooleanProperty("required");

        /// <inheritdoc/>
        public string RelatesTo => GetStringProperty("relatesTo");

        /// <inheritdoc/>
        public bool? Secret => GetBooleanProperty("secret");

        public IFormValue Form => GetResourceProperty<FormValue>("form");

        public IList<IFormValue> Options => GetArrayProperty<IFormValue>("options");

    }
}
