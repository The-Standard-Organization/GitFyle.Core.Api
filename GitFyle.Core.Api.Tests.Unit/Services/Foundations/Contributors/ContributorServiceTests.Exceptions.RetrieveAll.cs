// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageContributorException =
                new FailedStorageContributorException(
                    message: "Failed contributor storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                    innerException: failedStorageContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributorsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Contributor>> retrieveAllContributorsTask =
                this.contributorService.RetrieveAllContributorsAsync();

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    testCode: retrieveAllContributorsTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributorsAsync(),
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

        [Fact]
        public async Task ShouldThrowServiceErrorOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceError = new Exception();

            var failedServiceContributorException =
                new FailedServiceContributorException(
                    message: "Failed service contributor error occurred, contact support.",
                    innerException: serviceError);

            var expectedContributorServiceException =
                new ContributorServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributorsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Contributor>> retrieveAllContributorsTask =
                this.contributorService.RetrieveAllContributorsAsync();

            ContributorServiceException actualContributorServiceException =
                await Assert.ThrowsAsync<ContributorServiceException>(
                    testCode: retrieveAllContributorsTask.AsTask);

            // then
            actualContributorServiceException.Should().BeEquivalentTo(
                expectedContributorServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributorsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorServiceException))),
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