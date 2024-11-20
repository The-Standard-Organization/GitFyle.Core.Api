// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someContributorGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedContributorStorageException =
                new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                        innerException: failedContributorStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(someContributorGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contributor> removeContributorByIdTask =
                this.contributorService.RemoveContributorByIdAsync(someContributorGuid);

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    removeContributorByIdTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

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