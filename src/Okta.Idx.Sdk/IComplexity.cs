using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    public interface IComplexity
    {
        IList<string> ExcludeAttributes { get; }
        bool? ExcludeUserName { get; }
        int? MinLength { get; }
        int? MinLowerCase { get; }
        int? MinNumber { get; }
        int? MinSymbol { get; }
        int? MinUpperCase { get; }
    }
}