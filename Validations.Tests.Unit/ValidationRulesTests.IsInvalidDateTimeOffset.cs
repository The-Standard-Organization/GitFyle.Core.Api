// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations.Tests.Unit
{
    public partial class ValidationRulesTests
    {
        [Fact]
        public void IsInvalidDateTimeOffsetShouldReturnTrueForInvalidDateTimeOffset()
        {
            // Given
            DateTimeOffset defaultDate = default;

            // When
            dynamic result = ValidationRules.IsInvalid(defaultDate);

            // Then
            Assert.True(SafeGetDynamicProperty<bool>(result, "Condition"));
            Assert.Equal("Date is invalid", SafeGetDynamicProperty<string>(result, "Message"));
        }

        [Fact]
        public void IsInvalidDateTimeOffsetShouldReturnFalseForValidDateTimeOffset()
        {
            // Given
            DateTimeOffset validDate = DateTimeOffset.Now;

            // When
            dynamic result = ValidationRules.IsInvalid(validDate);

            // Then
            Assert.False(SafeGetDynamicProperty<bool>(result, "Condition"));
        }
    }
}
