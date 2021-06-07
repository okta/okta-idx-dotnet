using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    using Okta.Idx.Sdk.Helpers;

    public class IdxUrlHelperShould
    {
        [Fact]
        public void GetNormalizedUriStringWithoutTrailingSlashInArgument()
        {
            string normalized = IdxUrlHelper.GetNormalizedUriString("https://org.okta.com/oauth2/default", "the/resource");
            normalized.Should().Be("https://org.okta.com/oauth2/default/the/resource");
        }

        [Fact]
        public void GetNormalizedUriStringWithTrailingSlashInArgument()
        {
            string normalized = IdxUrlHelper.GetNormalizedUriString("https://org.okta.com/oauth2/default/", "the/resource");
            normalized.Should().Be("https://org.okta.com/oauth2/default/the/resource");
        }
    }
}
