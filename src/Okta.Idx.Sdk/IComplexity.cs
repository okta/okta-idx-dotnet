using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent complexity.
    /// </summary>
    public interface IComplexity
    {
        /// <summary>
        /// Gets the exclude attributes.
        /// </summary>
        IList<string> ExcludeAttributes { get; }

        /// <summary>
        /// Gets a value indicating whether to exclude the user name.
        /// </summary>
        bool? ExcludeUserName { get; }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        int? MinLength { get; }

        /// <summary>
        /// Gets the minimum lowercase.
        /// </summary>
        int? MinLowerCase { get; }

        /// <summary>
        /// Gets the minimum number.
        /// </summary>
        int? MinNumber { get; }

        /// <summary>
        /// Gets the minimum symbol.
        /// </summary>
        int? MinSymbol { get; }

        /// <summary>
        /// Gets the minimum uppercase.
        /// </summary>
        int? MinUpperCase { get; }
    }
}