// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations
{
    public static partial class ValidationRules
    {
        /// <summary>
        /// Compares two DateTimeOffsets to check if they are the same.
        /// </summary>
        /// <param name="first">The first DateTimeOffset to compare.</param>
        /// <param name="second">The second DateTimeOffset to compare.</param>
        /// <param name="secondName">The name of the second DateTimeOffset for the message.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the DateTimeOffsets are the same, otherwise <c>false</c>.
        /// - <c>Message</c>: "Date is not the same as {secondName}"
        /// </returns>
        public static dynamic IsSameAs(DateTimeOffset first, DateTimeOffset second, string secondName) => new
        {
            Condition = first == second,
            Message = $"Date is the same as {secondName}",
            Values = new object[] { first, second, secondName }
        };
    }
}
