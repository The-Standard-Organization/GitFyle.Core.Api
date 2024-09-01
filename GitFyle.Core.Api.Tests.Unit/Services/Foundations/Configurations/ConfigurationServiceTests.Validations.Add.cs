// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConfigurationIsNullAndLogItAsync()
        {
            // given
            Configuration nullConfiguration = null;

            var nullConfigurationException =
                new NullConfigurationException(
                    message: "Configuration is null");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix errors and try again",
                    innerException: nullConfigurationException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(nullConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfConfigurationIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            Configuration invalidConfiguration = new Configuration
            {
                Id = Guid.Empty,
                Name = invalidString,
                Value = invalidString,
                Type = invalidString,
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
                CreatedDate = default,
                UpdatedDate = default
            };

            var invalidConfigurationException = 
                new InvalidConfigurationException(
                    message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.Id),
                value: "Id is invalid."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.Name),
                value: "Text is required."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.Value),
                value: "Text is required."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.CreatedBy),
                value: "Text is required."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.UpdatedBy),
                value: "Text is required."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.CreatedDate),
                value: "Date is invalid."
                );

            invalidConfigurationException.Data.Add(
                key: nameof(Configuration.UpdatedDate),
                value: "Date is invalid."
                );

            var expectedConfigurationValidationException = 
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix errors and try again.", 
                    innerException: invalidConfigurationException);

            // when
            ValueTask<Configuration> addConfigurationTask = 
                this.configurationService.AddConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException = 
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should()
                .BeEquivalentTo(expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConfigurationValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertConfigurationAsync(invalidConfiguration), 
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
