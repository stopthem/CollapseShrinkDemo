using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowercaseNamingConvention(this string s, bool toLowercase)
        {
            if (toLowercase)
            {
                var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                return r.Replace(s, " ").ToLower();
            }
            else
                return s;
        }

        public static int NthIndexOf(this string s, string c, int n)
        {
            var takeCount = s.TakeWhile(x => (n -= (x.ToString() == c ? 1 : 0)) > 0).Count();
            return takeCount == s.Length ? -1 : takeCount;
        }
    }
}