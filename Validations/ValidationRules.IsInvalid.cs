// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations
{
    public static partial class ValidationRules
    {
        /// <summary>
        /// Validates a GUID to check if it is empty.
        /// </summary>
        /// <param name="id">The GUID to validate.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the GUID is empty, otherwise <c>false</c>.
        /// - <c>Message</c>: Id is invalid
        /// </returns>
        public static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        /// <summary>
        /// Validates a string to check if it is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="text">The string to validate.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the string is null, empty, or consists only of 
        ///                     white-space characters, otherwise <c>false</c>.
        /// - <c>Message</c>: "Text is required"
        /// </returns>
        public static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is invalid"
        };

        /// <summary>
        /// Validates a DateTimeOffset to check if it is the default value.
        /// </summary>
        /// <param name="date">The DateTimeOffset to validate.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the DateTimeOffset is the default value, otherwise <c>false</c>.
        /// - <c>Message</c>: "Date is invalid"
        /// </returns>
        public static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };
    }
}
