// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorIsNullAndLogItAsync()
        {
            // given
            Contributor nullContributor = null;

            var nullContributorException =
                new NullContributorException(message: "Contributor is null");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: nullContributorException);

            // when
            ValueTask<Contributor> addContributorTask =
                this.contributorService.ModifyContributorAsync(nullContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: addContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidContributor = new Contributor
            {
                Id = Guid.Empty,
                SourceId = Guid.Empty,
                Username = invalidString,
                Name = invalidString,
                Email = invalidString,
                AvatarUrl = invalidString,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
            };

            var invalidContributorException = new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.Id),
                values: "Id is invalid");

            invalidContributorException.AddData(
                 key: nameof(Contributor.SourceId),
                 values: "Id is invalid");

            invalidContributorException.AddData(
                key: nameof(Contributor.Username),
                values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.Name),
                values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.Email),
               values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.AvatarUrl),
               values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.CreatedBy),
               values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedBy),
                values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.CreatedDate),
                values: "Date is invalid");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values:
                    new[]
                    {
                      "Date is invalid",
                      $"Date is the same as {nameof(Contributor.CreatedDate)}"
                    });

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidContributor = CreateRandomContributor(dateTimeOffset: randomDateTimeOffset);
            invalidContributor.ExternalId = GetRandomStringWithLengthOf(256);
            invalidContributor.Username = GetRandomStringWithLengthOf(256);
            invalidContributor.Name = GetRandomStringWithLengthOf(256);
            invalidContributor.Email = GetRandomStringWithLengthOf(256);

            var invalidContributorException =
                new InvalidContributorException(
                    message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.ExternalId),
                values: $"Text exceeds max length of {invalidContributor.ExternalId.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Username),
                values: $"Text exceeds max length of {invalidContributor.Username.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Name),
                values: $"Text exceeds max length of {invalidContributor.Name.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Email),
                values: $"Text exceeds max length of {invalidContributor.Email.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values: $"Date is the same as {nameof(Contributor.CreatedDate)}");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should()
                .BeEquivalentTo(expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}