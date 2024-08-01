// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace Validations.Tests.Unit
{
    public partial class ValidationRulesTests
    {
        [Fact]
        public void IsInvalidGuidShouldReturnTrueForInvalidGuid()
        {
            // Given
            Guid emptyGuid = Guid.Empty;

            // When
            dynamic result = ValidationRules.IsInvalid(emptyGuid);

            // Then
            Assert.True(SafeGetDynamicProperty<bool>(result, "Condition"));
            Assert.Equal("Id is invalid", SafeGetDynamicProperty<string>(result, "Message"));
        }

        [Fact]
        public void IsInvalidGuidShouldReturnValseForValidGuid()
        {
            // Given
            Guid nonEmptyGuid = Guid.NewGuid();

            // When
            dynamic result = ValidationRules.IsInvalid(nonEmptyGuid);

            // Then
            Assert.False(SafeGetDynamicProperty<bool>(result, "Condition"));
        }
    }
}
