// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageContributionTypeException =
                new FailedStorageContributionTypeException(
                    message: "Failed contributionType storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                    innerException: failedStorageContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributionTypesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ContributionType>> retrieveAllContributionTypesTask =
                this.contributionTypeService.RetrieveAllContributionTypesAsync();

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    testCode: retrieveAllContributionTypesTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributionTypesAsync(),
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

        [Fact]
        public async Task ShouldThrowServiceErrorOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceError = new Exception();

            var failedServiceContributionTypeException =
                new FailedServiceContributionTypeException(
                    message: "Failed service contributionType error occurred, contact support.",
                    innerException: serviceError);

            var expectedContributionTypeServiceException =
                new ContributionTypeServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributionTypesAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<ContributionType>> retrieveAllContributionTypesTask =
                this.contributionTypeService.RetrieveAllContributionTypesAsync();

            ContributionTypeServiceException actualContributionTypeServiceException =
                await Assert.ThrowsAsync<ContributionTypeServiceException>(
                    testCode: retrieveAllContributionTypesTask.AsTask);

            // then
            actualContributionTypeServiceException.Should().BeEquivalentTo(
                expectedContributionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributionTypesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeServiceException))),
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