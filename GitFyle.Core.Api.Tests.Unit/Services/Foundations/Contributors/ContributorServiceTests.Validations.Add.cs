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
        public async Task ShouldThrowValidationExceptionOnAddIfContributorIsNullAndLogItAsync()
        {
            // given
            Contributor nullContributor = null;

            var nullContributorException =
                new NullContributorException(
                    message: "Contributor is null");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: nullContributorException);

            // when
            ValueTask<Contributor> addContributorTask =
                this.contributorService.AddContributorAsync(nullContributor);

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
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfContributorIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;

            var invalidContributor = new Contributor
            {
                Id = Guid.Empty,
                ExternalId = invalidString,
                SourceId = Guid.Empty,
                Username = invalidString,
                Name = invalidString,
                Email = invalidString,
                AvatarUrl = invalidString,
                ExternalCreatedAt = default,
                ExternalUpdatedAt = default,
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
                values: "Date is invalid");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> addContributorTask =
                this.contributorService.AddContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: addContributorTask.AsTask);

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
    }
}
