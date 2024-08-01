// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace Validations
{
    public static partial class ValidationRules
    {
        /// <summary>
        /// Validates if a string exceeds the specified maximum length.
        /// </summary>
        /// <param name="text">The string to validate.</param>
        /// <param name="maxLength">The maximum allowable length for the string.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the string exceeds the specified maximum length, otherwise <c>false</c>.
        /// - <c>Message</c>: A message indicating that the text exceeds the maximum length.
        /// </returns>
        public static dynamic IsInvalidMaxLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        /// <summary>
        /// Validates if a string is shorter than the specified minimum length.
        /// </summary>
        /// <param name="text">The string to validate.</param>
        /// <param name="minimumLength">The minimum allowable length for the string.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the string is shorter than the specified minimum length, otherwise <c>false</c>.
        /// - <c>Message</c>: A message indicating that the text must be at least the minimum length.
        /// </returns>
        public static dynamic IsInvalidMinimumLength(string text, int minimumLength) => new
        {
            Condition = IsLessThanLength(text, minimumLength),
            Message = $"Text must be a minimum length of {minimumLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;


        private static bool IsLessThanLength(string text, int minLength) =>
            (text ?? string.Empty).Length < minLength;
    }
}
