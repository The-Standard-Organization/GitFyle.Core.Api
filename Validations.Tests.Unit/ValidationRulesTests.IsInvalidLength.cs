// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace Validations.Tests.Unit
{
    public partial class ValidationRulesTests
    {
        [Fact]
        public void IsInvalidLengthShouldReturnTrueForInvalidMaxLength()
        {
            // Given
            string randomString = GetRandomString();
            int maxLength = randomString.Length - 1;

            // When
            dynamic result = ValidationRules.IsInvalidMaxLength(text: randomString, maxLength);

            // Then
            Assert.True(SafeGetDynamicProperty<bool>(result, "Condition"));
            Assert.Equal($"Text exceed max length of {maxLength} characters", SafeGetDynamicProperty<string>(result, "Message"));
        }

        [Fact]
        public void IsInvalidLengthShouldReturnFalseForValidMaxLength()
        {
            // Given
            string randomString = GetRandomString();
            int maxLength = randomString.Length;

            // When
            dynamic result = ValidationRules.IsInvalidMaxLength(randomString, maxLength);

            // Then
            Assert.False(SafeGetDynamicProperty<bool>(result, "Condition"));
        }

        [Fact]
        public void IsInvalidLengthShouldReturnTrueForInvalidMinLength()
        {
            // Given
            string randomString = GetRandomString();
            int minimumLength = randomString.Length + 1;

            // When
            dynamic result = ValidationRules.IsInvalidMinimumLength(text: randomString, minimumLength);

            // Then
            Assert.True(SafeGetDynamicProperty<bool>(result, "Condition"));

            Assert.Equal($"Text must be a minimum length of {minimumLength} characters",
                SafeGetDynamicProperty<string>(result, "Message"));
        }

        [Fact]
        public void IsInvalidLengthShouldReturnFalseForValidMinLength()
        {
            // Given
            string randomString = GetRandomString();
            int minimumLength = randomString.Length;

            // When
            dynamic result = ValidationRules.IsInvalidMinimumLength(randomString, minimumLength);

            // Then
            Assert.False(SafeGetDynamicProperty<bool>(result, "Condition"));
        }
    }
}
