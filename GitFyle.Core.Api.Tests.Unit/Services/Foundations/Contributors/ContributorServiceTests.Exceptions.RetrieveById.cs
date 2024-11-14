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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            Guid someGuid = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageContributorException =
                new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                    innerException: failedStorageContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contributor> retrieveContributorByIdTask =
                this.contributorService.RetrieveContributorByIdAsync(someGuid);

            // then
            await Assert.ThrowsAsync<ContributorDependencyException>(
                testCode: retrieveContributorByIdTask.AsTask);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyException))),
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