﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace System;

public static class StringExtensions
{
    private static readonly CultureInfo enUSCultInfo = new CultureInfo("en-US", false);

    public static string MD5(this string s)
    {
        var builder = new StringBuilder();
        foreach (byte b in MD5Bytes(s))
        {
            builder.Append(b.ToString("x2").ToLower());
        }

        return builder.ToString();
    }

    public static byte[] MD5Bytes(this string s)
    {
        using (var provider = System.Security.Cryptography.MD5.Create())
        {
            return provider.ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }

    public static string ConvertToSortableName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        var newName = name;
        newName = Regex.Replace(newName, @"^the\s+", "", RegexOptions.IgnoreCase);
        newName = Regex.Replace(newName, @"^a\s+", "", RegexOptions.IgnoreCase);
        newName = Regex.Replace(newName, @"^an\s+", "", RegexOptions.IgnoreCase);
        return newName;
    }

    public static string RemoveTrademarks(this string str, string replacement = "")
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }

        return Regex.Replace(str, @"[™©®]", replacement);
    }

    public static bool IsNullOrEmpty(this string source)
    {
        return string.IsNullOrEmpty(source);
    }

    public static bool IsNullOrWhiteSpace(this string source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    public static string Format(this string source, params object[] args)
    {
        return string.Format(source, args);
    }

    public static string TrimEndString(this string source, string value, StringComparison comp = StringComparison.Ordinal)
    {
        if (!source.EndsWith(value, comp))
        {
            return source;
        }

        return source.Remove(source.LastIndexOf(value, comp));
    }

    public static string ToTitleCase(this string source, CultureInfo culture = null)
    {
        if (source.IsNullOrEmpty())
        {
            return source;
        }

        if (culture != null)
        {
            return culture.TextInfo.ToTitleCase(source);
        }
        else
        {
            return enUSCultInfo.TextInfo.ToTitleCase(source);
        }
    }

    private static string RemoveUnlessThatEmptiesTheString(string input, string pattern)
    {
        string output = Regex.Replace(input, pattern, string.Empty);

        if (string.IsNullOrWhiteSpace(output))
        {
            return input;
        }
        return output;
    }

    public static string NormalizeGameName(this string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        var newName = name;
        newName = newName.RemoveTrademarks();
        newName = newName.Replace("_", " ");
        newName = newName.Replace(".", " ");
        newName = newName.Replace('’', '\'');
        newName = RemoveUnlessThatEmptiesTheString(newName, @"\[.*?\]");
        newName = RemoveUnlessThatEmptiesTheString(newName, @"\(.*?\)");
        newName = Regex.Replace(newName, @"\s*:\s*", ": ");
        newName = Regex.Replace(newName, @"\s+", " ");
        if (Regex.IsMatch(newName, @",\s*The$"))
        {
            newName = "The " + Regex.Replace(newName, @",\s*The$", "", RegexOptions.IgnoreCase);
        }

        return newName.Trim();
    }

    public static string GetSHA256Hash(this string input)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    public static string GetPathWithoutAllExtensions(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        return Regex.Replace(path, @"(\.[A-Za-z0-9]+)+$", "");
    }

    public static bool Contains(this string str, string value, StringComparison comparisonType)
    {
        return str?.IndexOf(value, 0, comparisonType) != -1;
    }

    public static bool ContainsAny(this string str, char[] chars)
    {
        return str?.IndexOfAny(chars) >= 0;
    }

    public static bool IsHttpUrl(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        return Regex.IsMatch(str, @"^https?:\/\/", RegexOptions.IgnoreCase);
    }

    public static bool IsUri(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        return Uri.IsWellFormedUriString(str, UriKind.Absolute);
    }

    public static string UrlEncode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return HttpUtility.UrlPathEncode(str);
    }

    public static string UrlDecode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return HttpUtility.UrlDecode(str);
    }

    public static string Base64Encode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    public static string Base64Decode(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(str));
    }

    public static string GetMatchModifiedName(this string str)
    {
        return Regex.Replace(str, @"[^\p{L}\p{Nd}]", "").ToLower();
    }

    // Courtesy of https://stackoverflow.com/questions/6275980/string-replace-ignoring-case
    public static string Replace(this string str, string oldValue, string @newValue, StringComparison comparisonType)
    {
        // Check inputs.
        if (str == null)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(nameof(str));
        }
        if (str.Length == 0)
        {
            // Same as original .NET C# string.Replace behavior.
            return str;
        }
        if (oldValue == null)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(nameof(oldValue));
        }
        if (oldValue.Length == 0)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentException("String cannot be of zero length.");
        }

        // Prepare string builder for storing the processed string.
        // Note: StringBuilder has a better performance than String by 30-40%.
        StringBuilder resultStringBuilder = new StringBuilder(str.Length);

        // Analyze the replacement: replace or remove.
        bool isReplacementNullOrEmpty = string.IsNullOrEmpty(@newValue);

        // Replace all values.
        const int valueNotFound = -1;
        int foundAt;
        int startSearchFromIndex = 0;
        while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
        {
            // Append all characters until the found replacement.
            int @charsUntilReplacment = foundAt - startSearchFromIndex;
            bool isNothingToAppend = @charsUntilReplacment == 0;
            if (!isNothingToAppend)
            {
                resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilReplacment);
            }

            // Process the replacement.
            if (!isReplacementNullOrEmpty)
            {
                resultStringBuilder.Append(@newValue);
            }

            // Prepare start index for the next search.
            // This needed to prevent infinite loop, otherwise method always start search
            // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
            // and comparisonType == "any ignore case" will conquer to replacing:
            // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
            startSearchFromIndex = foundAt + oldValue.Length;
            if (startSearchFromIndex == str.Length)
            {
                // It is end of the input string: no more space for the next search.
                // The input string ends with a value that has already been replaced.
                // Therefore, the string builder with the result is complete and no further action is required.
                return resultStringBuilder.ToString();
            }
        }

        // Append the last part to the result.
        int @charsUntilStringEnd = str.Length - startSearchFromIndex;
        resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilStringEnd);
        return resultStringBuilder.ToString();
    }

    public static bool ContainsInvariantCulture(this string source, string value, CompareOptions compareOptions)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, value, compareOptions) >= 0;
    }

    public static bool ContainsCurrentCulture(this string source, string value, CompareOptions compareOptions)
    {
        return CultureInfo.CurrentCulture.CompareInfo.IndexOf(source, value, compareOptions) >= 0;
    }

    //From https://github.com/DanHarltey/Fastenshtein
    /// <summary>
    /// Compares the two values to find the minimum Levenshtein distance. 
    /// Thread safe.
    /// </summary>
    /// <returns>Difference. 0 complete match.</returns>
    public static int GetLevenshteinDistance(this string value1, string value2)
    {
        if (value2.Length == 0)
        {
            return value1.Length;
        }

        int[] costs = new int[value2.Length];

        // Add indexing for insertion to first row
        for (int i = 0; i < costs.Length;)
        {
            costs[i] = ++i;
        }

        for (int i = 0; i < value1.Length; i++)
        {
            // cost of the first index
            int cost = i;
            int previousCost = i;

            // cache value for inner loop to avoid index lookup and bonds checking, profiled this is quicker
            char value1Char = value1[i];

            for (int j = 0; j < value2.Length; j++)
            {
                int currentCost = cost;
                cost = costs[j];

                if (value1Char != value2[j])
                {
                    if (previousCost < currentCost)
                    {
                        currentCost = previousCost;
                    }

                    if (cost < currentCost)
                    {
                        currentCost = cost;
                    }

                    ++currentCost;
                }

                costs[j] = currentCost;
                previousCost = currentCost;
            }
        }

        return costs[costs.Length - 1];
    }
}