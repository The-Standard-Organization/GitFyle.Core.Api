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

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            //given
            var someContributionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceContributionException =
                new FailedServiceContributionException(
                    message: "Failed service contribution error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionServiceException =
                new ContributionServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<Contribution> retrieveContributionByIdTask =
                this.contributionService.RetrieveContributionByIdAsync(someContributionId);

            ContributionServiceException actualContributionServiceException =
                await Assert.ThrowsAsync<ContributionServiceException>(
                    retrieveContributionByIdTask.AsTask);

            //then
            actualContributionServiceException.Should().BeEquivalentTo(
                expectedContributionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogErrorAsync(It.Is(SameExceptionAs(
                expectedContributionServiceException))),
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