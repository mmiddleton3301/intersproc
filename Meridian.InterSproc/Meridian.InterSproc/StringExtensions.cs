// ----------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="MTCS">
// Copyright (c) MTCS 2018.
// MTCS is a trading name of Meridian Technology Consultancy Services Ltd.
// Meridian Technology Consultancy Services Ltd is registered in England and
// Wales. Company number: 11184022.
// </copyright>
// ----------------------------------------------------------------------------

namespace Meridian.InterSproc
{
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Static extensions class for the <see cref="string" /> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the first character in a string to uppercase.
        /// </summary>
        /// <param name="input">
        /// The <see cref="string" /> to change.
        /// </param>
        /// <returns>
        /// The converted <see cref="string" /> value.
        /// </returns>
        public static string FirstCharToUpper(this string input)
        {
            string toReturn = new string(new char[] { input.First() })
                .ToUpper(CultureInfo.InvariantCulture);

            toReturn += input.Substring(1);

            return toReturn;
        }
    }
}