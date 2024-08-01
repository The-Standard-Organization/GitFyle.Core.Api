// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Services.Foundations.Sources;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public void IsInvalidUrlShouldReturnTrueForInvalidUrl()
        {
            // Given
            string invalidUrl = "invalid_url";

            // When
            dynamic result = SourceService.IsInvalidUrl(invalidUrl);

            // Then
            Assert.True(result.Condition);
            Assert.Equal("Url is invalid", result.Message);
        }

        [Fact]
        public void IsInvalidUrlShouldReturnFalseForValidUrl()
        {
            // Given
            string validUrl = "http://www.example.com";

            // When
            dynamic result = SourceService.IsInvalidUrl(validUrl);

            // Then
            Assert.False(result.Condition);
        }
    }
}
