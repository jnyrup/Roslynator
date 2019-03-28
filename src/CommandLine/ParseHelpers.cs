﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class ParseHelpers
    {
        private static readonly Regex _lowerLetterUpperLetterRegex = new Regex(@"\p{Ll}\p{Lu}");

        public static bool TryParseMSBuildProperties(IEnumerable<string> values, out Dictionary<string, string> properties)
        {
            properties = null;

            foreach (string property in values)
            {
                int index = property.IndexOf("=");

                if (index == -1)
                {
                    WriteLine($"Unable to parse property '{property}'", Verbosity.Quiet);
                    return false;
                }

                string key = property.Substring(0, index);

                if (properties == null)
                    properties = new Dictionary<string, string>();

                properties[key] = property.Substring(index + 1);
            }

            if (properties?.Count > 0)
            {
                int maxLength = properties.Max(f => f.Key.Length);

                foreach (KeyValuePair<string, string> kvp in properties)
                    WriteLine($"Add MSBuild property {kvp.Key.PadRight(maxLength)} = {kvp.Value}", ConsoleColor.DarkGray, Verbosity.Detailed);
            }

            return true;
        }

        public static bool TryParseKeyValuePairs(IEnumerable<string> values, out List<KeyValuePair<string, string>> properties)
        {
            properties = null;

            foreach (string property in values)
            {
                int index = property.IndexOf("=");

                if (index == -1)
                {
                    WriteLine($"Unable to parse key/value pair '{property}'", Verbosity.Quiet);
                    return false;
                }

                string key = property.Substring(0, index);
                string value = property.Substring(index + 1);

                (properties ?? (properties = new List<KeyValuePair<string, string>>())).Add(new KeyValuePair<string, string>(key, value));
            }

            return true;
        }

        public static bool TryParseOptionValueAsEnumFlags<TEnum>(
            IEnumerable<string> values,
            string optionName,
            out TEnum result,
            TEnum? defaultValue = null) where TEnum : struct
        {
            result = (TEnum)(object)0;

            if (values?.Any() != true)
            {
                if (defaultValue != null)
                {
                    result = (TEnum)(object)defaultValue;
                }

                return true;
            }

            int flags = 0;

            foreach (string value in values)
            {
                if (!TryParseOptionValueAsEnum(value, optionName, out TEnum result2))
                    return false;

                flags |= (int)(object)result2;
            }

            result = (TEnum)(object)flags;

            return true;
        }

        public static bool TryParseOptionValueAsEnumValues<TEnum>(
            IEnumerable<string> values,
            string optionName,
            out ImmutableArray<TEnum> result,
            ImmutableArray<TEnum> defaultValue = default) where TEnum : struct
        {
            if (values?.Any() != true)
            {
                result = (defaultValue.IsDefault) ? ImmutableArray<TEnum>.Empty : defaultValue;

                return true;
            }

            ImmutableArray<TEnum>.Builder builder = ImmutableArray.CreateBuilder<TEnum>();

            foreach (string value in values)
            {
                if (!TryParseOptionValueAsEnum(value, optionName, out TEnum result2))
                    return false;

                builder.Add(result2);
            }

            result = builder.ToImmutableArray();

            return true;
        }

        public static bool TryParseOptionValueAsEnum<TEnum>(string value, string optionName, out TEnum result, TEnum? defaultValue = null) where TEnum : struct
        {
            if (value == null
                && defaultValue != null)
            {
                result = defaultValue.Value;
                return true;
            }

            if (!Enum.TryParse(value?.Replace("-", ""), ignoreCase: true, out result))
            {
                IEnumerable<string> values = Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Select(f => _lowerLetterUpperLetterRegex.Replace(f.ToString(), e => e.Value.Insert(1, "-")).ToLowerInvariant());

                WriteLine($"Option '--{optionName}' has unknown value '{value}'. Known values: {string.Join(", ", values)}.", Verbosity.Quiet);
                return false;
            }

            return true;
        }

        public static bool TryParseVerbosity(string value, out Verbosity verbosity)
        {
            switch (value)
            {
                case "q":
                    {
                        verbosity = Verbosity.Quiet;
                        return true;
                    }
                case "m":
                    {
                        verbosity = Verbosity.Minimal;
                        return true;
                    }
                case "n":
                    {
                        verbosity = Verbosity.Normal;
                        return true;
                    }
                case "d":
                    {
                        verbosity = Verbosity.Detailed;
                        return true;
                    }
                case "diag":
                    {
                        verbosity = Verbosity.Diagnostic;
                        return true;
                    }
            }

            if (Enum.TryParse(value, ignoreCase: true, out verbosity))
                return true;

            WriteLine($"Unknown verbosity '{value}'.", Verbosity.Quiet);
            return false;
        }

        public static bool TryParseLanguage(string value, out string language)
        {
            switch (value)
            {
                case "cs":
                case "csharp":
                    {
                        language = LanguageNames.CSharp;
                        return true;
                    }
                case "vb":
                case "visual-basic":
                    {
                        language = LanguageNames.VisualBasic;
                        return true;
                    }
            }

            WriteLine($"Unknown language '{value}'.", Verbosity.Quiet);

            language = null;
            return false;
        }

        public static bool TryParseMetadataNames(IEnumerable<string> values, out ImmutableArray<MetadataName> metadataNames)
        {
            ImmutableArray<MetadataName>.Builder builder = null;

            foreach (string value in values)
            {
                if (!MetadataName.TryParse(value, out MetadataName metadataName))
                {
                    WriteLine($"Unable to parse metadata name '{value}'.", Verbosity.Quiet);
                    return false;
                }

                (builder ?? (builder = ImmutableArray.CreateBuilder<MetadataName>())).Add(metadataName);
            }

            metadataNames = builder?.ToImmutableArray() ?? ImmutableArray<MetadataName>.Empty;

            return true;
        }
    }
}
