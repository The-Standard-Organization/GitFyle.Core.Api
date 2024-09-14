// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using System;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someSourceGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedSourceStorageException =
                new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                        innerException: failedSourceStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(someSourceGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Source> removeByIdSourceTask =
                this.sourceService.RemoveSourceByIdAsync(someSourceGuid);

            SourceDependencyException actualSourceDependencyException =
                await Assert.ThrowsAsync<SourceDependencyException>(
                    removeByIdSourceTask.AsTask);

            // then
            actualSourceDependencyException.Should().BeEquivalentTo(
                expectedSourceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyException))),
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