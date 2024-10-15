// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
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

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            //given
            var someContributionTypeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceContributionTypeException =
                new FailedServiceContributionTypeException(
                    message: "Failed service contributionType error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionTypeServiceException =
                new ContributionTypeServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<ContributionType> retrieveContributionTypeByIdTask =
                this.contributionTypeService.RetrieveContributionTypeByIdAsync(someContributionTypeId);

            ContributionTypeServiceException actualContributionTypeServiceException =
                await Assert.ThrowsAsync<ContributionTypeServiceException>(
                    testCode: retrieveContributionTypeByIdTask.AsTask);

            //then
            actualContributionTypeServiceException.Should().BeEquivalentTo(
                expectedContributionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(It.IsAny<Guid>()),
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