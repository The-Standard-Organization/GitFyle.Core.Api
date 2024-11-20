// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
    }
}