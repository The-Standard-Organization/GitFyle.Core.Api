// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            Guid someGuid = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageContributionTypeException =
                new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                    innerException: failedStorageContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ContributionType> retrieveContributionTypeByIdTask =
                this.contributionTypeService.RetrieveContributionTypeByIdAsync(someGuid);

            // then
            await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                testCode: retrieveContributionTypeByIdTask.AsTask);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyException))),
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