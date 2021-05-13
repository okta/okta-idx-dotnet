using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    public class IdxUrlHelperShould
    {
        [Fact]
        public async Task GetNormalizedUriStringWithoutTrailingSlashInArgument()
        {
            string normalized = IdxUrlHelper.GetNormalizedUriString("https://org.okta.com/oauth2/default", "the/resource");
            normalized.Should().Be("https://org.okta.com/oauth2/default/the/resource");
        }

        [Fact]
        public async Task GetNormalizedUriStringWithTrailingSlashInArgument()
        {
            string normalized = IdxUrlHelper.GetNormalizedUriString("https://org.okta.com/oauth2/default/", "the/resource");
            normalized.Should().Be("https://org.okta.com/oauth2/default/the/resource");
        }
    }
}
