// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogitAsync()
        {
            // given
            Guid someRepositoryId = Guid.Empty;

            var invalidRepositoryException =
                new InvalidRepositoryException(
                    message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Id),
                values: "Id is invalid");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            // when
            ValueTask<Repository> removeRepositoryByIdTask =
                this.repositoryService.RemoveRepositoryByIdAsync(someRepositoryId);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    removeRepositoryByIdTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertRepositoryAsync(It.IsAny<Repository>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}