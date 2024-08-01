// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations
{
    public static partial class ValidationRules
    {
        /// <summary>
        /// Compares two strings to check if they are not the same.
        /// </summary>
        /// <param name="first">The first string to compare.</param>
        /// <param name="second">The second string to compare.</param>
        /// <param name="secondName">The name of the second string for the message.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the strings are not the same, otherwise <c>false</c>.
        /// - <c>Message</c>: "Text is not the same as {secondName}"
        /// </returns>
        public static dynamic IsNotSameAs(string first, string second, string secondName) => new
        {
            Condition = first != second,
            Message = $"Text is not the same as {secondName}",
            Values = new object[] { first, second, secondName }
        };

        /// <summary>
        /// Compares two DateTimeOffsets to check if they are not the same.
        /// </summary>
        /// <param name="first">The first DateTimeOffset to compare.</param>
        /// <param name="second">The second DateTimeOffset to compare.</param>
        /// <param name="secondName">The name of the second DateTimeOffset for the message.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the DateTimeOffsets are not the same, otherwise <c>false</c>.
        /// - <c>Message</c>: "Date is not the same as {secondName}"
        /// </returns>
        public static dynamic IsNotSameAs(DateTimeOffset first, DateTimeOffset second, string secondName) => new
        {
            Condition = first != second,
            Message = $"Date is not the same as {secondName}",
            Values = new object[] { first, second, secondName }
        };
    }
}
