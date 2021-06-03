using System;
using System.Collections.Generic;

namespace Okta.Idx.Sdk.Extensions
{
    public static class StringExtensions
    {
        public static List<string> ToList(this string stringParam,
            char separator = ' ')
        {
            var listOfStrings = new List<string>();

            if (!string.IsNullOrEmpty(stringParam))
            {
                listOfStrings = new List<string>(stringParam.Split(separator));
            }

            return listOfStrings;
        }
    }
}