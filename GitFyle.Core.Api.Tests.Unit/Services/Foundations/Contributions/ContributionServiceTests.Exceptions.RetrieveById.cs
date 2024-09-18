// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            var someContributionId = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageContributionException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                    innerException: failedStorageContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionId))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<Contribution> retrieveContributionByIdTask =
                this.contributionService.RetrieveContributionByIdAsync(someContributionId);

            // then
            await Assert.ThrowsAsync<ContributionDependencyException>(() =>
                retrieveContributionByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(someContributionId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}