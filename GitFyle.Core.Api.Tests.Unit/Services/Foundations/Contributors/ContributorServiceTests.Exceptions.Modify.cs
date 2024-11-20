// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Contributor someContributor = CreateRandomContributor();
            SqlException sqlException = CreateSqlException();

            var failedContributorStorageException =
                new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                        innerException: failedContributorStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(someContributor);

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(someContributor.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributorAsync(someContributor),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}