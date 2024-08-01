// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations
{
    public static partial class ValidationRules
    {
        /// <summary>
        /// Checks if a DateTimeOffset is not recent compared to the current date and time.
        /// </summary>
        /// <param name="date">The DateTimeOffset to check.</param>
        /// <param name="currentDateTime">The current date and time for comparison.</param>
        /// <returns>
        /// A dynamic containing a boolean and a string:
        /// - <c>Condition</c>: <c>true</c> if the DateTimeOffset is not recent, otherwise <c>false</c>.
        /// - <c>Message</c>: "Date is not recent"
        /// </returns>
        public static dynamic IsNotRecent(DateTimeOffset date, DateTimeOffset currentDateTime) => new
        {
            Condition = IsDateNotRecent(date, currentDateTime),
            Message = "Date is not recent"
        };

        private static bool IsDateNotRecent(DateTimeOffset date, DateTimeOffset currentDateTime)
        {
            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }
    }
}
