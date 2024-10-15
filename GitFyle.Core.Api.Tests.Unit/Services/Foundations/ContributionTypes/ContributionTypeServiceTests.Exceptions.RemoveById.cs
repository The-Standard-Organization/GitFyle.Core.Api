// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someContributionTypeGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedContributionTypeStorageException =
                new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                        innerException: failedContributionTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(someContributionTypeGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ContributionType> removeContributionTypeByIdTask =
                this.contributionTypeService.RemoveContributionTypeByIdAsync(someContributionTypeGuid);

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    removeContributionTypeByIdTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

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