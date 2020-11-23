using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public class IdxSuccessResponse : Resource, IIdxSuccessResponse
    {
        public IList<string> Rel => GetArrayProperty<string>("rel");

        public string Name => GetStringProperty("name");

        public string Method => GetStringProperty("method");

        public string Href => GetStringProperty("href");

        public string Accepts => GetStringProperty("accepts");

        public async Task<IResource> ExchangeCodeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
