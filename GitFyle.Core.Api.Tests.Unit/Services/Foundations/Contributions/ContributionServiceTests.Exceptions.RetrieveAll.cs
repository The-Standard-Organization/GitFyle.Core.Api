// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageContributionException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                    innerException: failedStorageContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributionsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Contribution>> retrieveAllContributionsTask =
                this.contributionService.RetrieveAllContributionsAsync();

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    testCode: retrieveAllContributionsTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributionsAsync(),
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