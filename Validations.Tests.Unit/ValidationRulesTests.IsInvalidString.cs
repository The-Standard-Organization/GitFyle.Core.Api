// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace Validations.Tests.Unit
{
    public partial class ValidationRulesTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t \n\r")]
        public void IsInvalidStringShouldReturnTrueForInvalidString(string invalidString)
        {
            // Given
            string emptyString = string.Empty;

            // When
            dynamic result = ValidationRules.IsInvalid(text: invalidString);

            // Then
            Assert.True(SafeGetDynamicProperty<bool>(result, "Condition"));
            Assert.Equal("Text is invalid", SafeGetDynamicProperty<string>(result, "Message"));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("test")]
        [InlineData(" a ")]
        [InlineData("  a")]
        [InlineData("a  ")]
        public void IsInvalidStringShouldReturnFalseForValidString(string validString)
        {
            // Given
            string nonEmptyString = GetRandomString();

            // When
            dynamic result = ValidationRules.IsInvalid(text: validString);

            // Then
            Assert.False(SafeGetDynamicProperty<bool>(result, "Condition"));
        }
    }
}
