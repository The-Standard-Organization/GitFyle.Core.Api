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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdWhenContributorIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidContributorId = Guid.Empty;

            var invalidContributorException =
                new InvalidContributorException(
                    message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.Id),
                values: "Id is invalid");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            // when
            ValueTask<Contributor> retrieveContributorByIdTask =
                this.contributorService.RetrieveContributorByIdAsync(invalidContributorId);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: retrieveContributorByIdTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}