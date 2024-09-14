// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                    message: "Configuration validation error occurred, fix the errors and try again.",
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
            DateTimeOffset randomDateTimeOffset = default;

            Configuration invalidConfiguration = new Configuration
            {
                Id = Guid.Empty,
                Name = invalidString,
                Value = invalidString,
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset
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
                values: "Date is invalid.");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

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
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesAreNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Configuration randomConfiguration = CreateRandomConfiguration(now);
            Configuration invalidConfiguration = randomConfiguration;
            invalidConfiguration.CreatedBy = GetRandomString();
            invalidConfiguration.UpdatedBy = GetRandomString();
            invalidConfiguration.CreatedDate = now;
            invalidConfiguration.UpdatedDate = GetRandomDateTimeOffset();

            InvalidConfigurationException invalidConfigurationException = new InvalidConfigurationException(
                    message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.UpdatedBy),
                values: $"Text is not same as {nameof(Configuration.CreatedBy)}");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.UpdatedDate),
                values: $"Date is not same as {nameof(Configuration.CreatedDate)}");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

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
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTime = 
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Configuration randomConfiguration = CreateRandomConfiguration();
            Configuration invalidConfiguration = randomConfiguration;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidConfiguration.CreatedDate = invalidDate;
            invalidConfiguration.UpdatedDate = invalidDate;

            var invalidConfigurationException = new InvalidConfigurationException(
                message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.CreatedDate),
                values: $"Date is not recent. Expected a value between {startDate} and {endDate}" +
                $" but found {invalidDate}");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            //when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(invalidConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(addConfigurationTask.AsTask);

            //then
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
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
