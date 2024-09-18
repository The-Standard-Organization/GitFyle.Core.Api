// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using System;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someContributionGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedContributionStorageException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                        innerException: failedContributionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contribution> removeContributionByIdTask =
                this.contributionService.RemoveContributionByIdAsync(someContributionGuid);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    removeContributionByIdTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
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