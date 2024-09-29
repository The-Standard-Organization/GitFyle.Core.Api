using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldThrowValidationExceptionOnModifyIfConfigurationIsNullAndLogItAsync()
        {
            // given
            Configuration nullConfiguration = null;

            var nullConfigurationException =
                new NullConfigurationException(
                    message: "Configuration is null");

            ConfigurationValidationException expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: nullConfigurationException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(nullConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfConfigurationIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            Configuration invalidConfiguration = new Configuration
            {
                Id = Guid.Empty,
                Name = invalidText,
                Value = invalidText,
                CreatedBy = invalidText,
                CreatedDate = default,
                UpdatedBy = invalidText,
                UpdatedDate = default
            };

            var invalidConfigurationException =
                new InvalidConfigurationException(
                    message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Id),
                values: "Id is invalid.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Name),
                values: "Text is required.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Value),
                values: "Text is required.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.CreatedBy),
                values: "Text is required.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.CreatedDate),
                values: "Date is invalid.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.UpdatedBy),
                values: "Text is required.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.UpdatedDate),
                values: new[] 
                    {
                        "Date is invalid.", 
                        $"Date is same as {nameof(Configuration.CreatedDate)}"
                    });

            ConfigurationValidationException expectedConfigurationValidationException = 
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            // when
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(invalidConfiguration);

            // then
            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    modifyConfigurationTask.AsTask);

            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConfigurationValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConfigHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Configuration randomConfiguration = CreateRandomModifyConfiguration(dateTimeOffset: randomDateTimeOffset);
            Configuration invalidConfiguration = randomConfiguration;
            invalidConfiguration.Name = GetRandomStringWithLengthOf(451);

            var invalidConfigurationException = new InvalidConfigurationException(
                message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Name),
                values: $"Text exceed max length of {invalidConfiguration.Name.Length - 1} characters");

            ConfigurationValidationException expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            // when
            ValueTask<Configuration> modifyConfigurationTask = 
                this.configurationService.ModifyConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should()
                .BeEquivalentTo(expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
