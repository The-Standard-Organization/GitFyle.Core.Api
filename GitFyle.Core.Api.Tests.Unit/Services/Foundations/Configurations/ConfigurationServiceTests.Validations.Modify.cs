using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Moq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
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

            this.datetimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should()
                .BeEquivalentTo(expectedConfigurationValidationException);

            this.datetimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffsetAsync(), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Configuration randomConfiguration = CreateRandomConfiguration(randomDateTimeOffset);
            randomConfiguration.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidConfigurationException =
                new InvalidConfigurationException(
                message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.UpdatedDate),
                values: $"Date is not recent." +
                $" Expected a value between {startDate} and {endDate} but found {randomConfiguration.UpdatedDate}");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(randomConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should()
                .BeEquivalentTo(expectedConfigurationValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageConfigurationDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Configuration randomConfiguration = CreateRandomConfiguration(randomDateTimeOffset);
            Configuration nonExistingConfiguration = randomConfiguration;
            nonExistingConfiguration.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Configuration nullConfiguration = null;

            var notFoundConfigurationException =
                new NotFoundConfigurationException(
                    message: $"Configuration not found with id: {nonExistingConfiguration.Id}");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: notFoundConfigurationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(nonExistingConfiguration.Id))
                    .ReturnsAsync(nullConfiguration);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(nonExistingConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should()
                .BeEquivalentTo(expectedConfigurationValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(nonExistingConfiguration.Id),
                    Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
