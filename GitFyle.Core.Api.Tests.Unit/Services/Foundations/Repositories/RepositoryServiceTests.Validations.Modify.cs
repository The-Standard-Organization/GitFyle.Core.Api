// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfRepositoryIsNullAndLogItAsync()
        {
            // given
            Repository nullRepository = null;

            var nullRepositoryException =
                new NullRepositoryException(message: "Repository is null");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: nullRepositoryException);

            // when
            ValueTask<Repository> addRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(nullRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: addRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertRepositoryAsync(It.IsAny<Repository>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfRepositoryIsInvalidAndLogItAsync(
                string invalidString)
        {
            // given
            var invalidRepository = new Repository
            {
                Id = Guid.Empty,
                Name = invalidString,
                Owner = invalidString,
                ExternalId = invalidString,
                SourceId = Guid.Empty,
                Token = invalidString,
                TokenExpireAt = default,
                Description = invalidString,
                ExternalCreatedAt = default,
                ExternalUpdatedAt = default,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
            };

            var invalidRepositoryException = new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Id),
                values: "Id is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Name),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Owner),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.ExternalId),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.SourceId),
                values: "Id is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Token),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.TokenExpireAt),
                values: "Date is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Description),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.ExternalCreatedAt),
                values: "Date is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.ExternalUpdatedAt),
                values: "Date is invalid");

            invalidRepositoryException.AddData(
               key: nameof(Repository.CreatedBy),
               values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedBy),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.CreatedDate),
                values: "Date is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(Repository.CreatedDate)}"
                    });

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertRepositoryAsync(It.IsAny<Repository>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfRepositoryHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidRepository = CreateRandomRepository(dateTimeOffset: randomDateTimeOffset);
            invalidRepository.Name = GetRandomStringWithLengthOf(256);
            invalidRepository.Owner = GetRandomStringWithLengthOf(256);
            invalidRepository.ExternalId = GetRandomStringWithLengthOf(256);

            var invalidRepositoryException =
                new InvalidRepositoryException(
                    message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Name),
                values: $"Text exceeds max length of {invalidRepository.Name.Length - 1} characters");

            invalidRepositoryException.AddData(
                key: nameof(Repository.Owner),
                values: $"Text exceeds max length of {invalidRepository.Owner.Length - 1} characters");

            invalidRepositoryException.AddData(
                key: nameof(Repository.ExternalId),
                values: $"Text exceeds max length of {invalidRepository.ExternalId.Length - 1} characters");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values: $"Date is the same as {nameof(Repository.CreatedDate)}");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should()
                .BeEquivalentTo(expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertRepositoryAsync(It.IsAny<Repository>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Repository randomRepository = CreateRandomRepository(randomDateTimeOffset);
            Repository invalidRepository = randomRepository;

            var invalidRepositoryException = new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values: $"Date is the same as {nameof(Repository.CreatedDate)}");

            var expectedRepositoryValidationException = new RepositoryValidationException(
                message: "Repository validation error occurred, fix errors and try again.",
                innerException: invalidRepositoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
             int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Repository randomRepository = CreateRandomRepository(randomDateTimeOffset);
            randomRepository.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidRepositoryException = new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomRepository.UpdatedDate}"
                ]);

            var expectedRepositoryValidationException = new RepositoryValidationException(
                message: "Repository validation error occurred, fix errors and try again.",
                innerException: invalidRepositoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(randomRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageRepositoryDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Repository randomRepository = CreateRandomRepository(randomDateTimeOffset);
            Repository nonExistingRepository = randomRepository;
            nonExistingRepository.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Repository nullRepository = null;

            var notFoundRepositoryException =
                new NotFoundRepositoryException(
                    message: $"Repository not found with id: {nonExistingRepository.Id}");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: notFoundRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(nonExistingRepository.Id))
                    .ReturnsAsync(nullRepository);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(nonExistingRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(nonExistingRepository.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Repository randomRepository = CreateRandomModifyRepository(randomDateTimeOffset);
            Repository invalidRepository = randomRepository;
            Repository storedRepository = randomRepository.DeepClone();
            storedRepository.CreatedBy = GetRandomString();
            storedRepository.CreatedDate = storedRepository.CreatedDate.AddMinutes(randomMinutes);
            storedRepository.UpdatedDate = storedRepository.UpdatedDate.AddMinutes(randomMinutes);
            Guid RepositoryId = invalidRepository.Id;

            var invalidRepositoryException = new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.CreatedBy),
                values: $"Text is not the same as {nameof(Repository.CreatedBy)}");

            invalidRepositoryException.AddData(
                key: nameof(Repository.CreatedDate),
                values: $"Date is not the same as {nameof(Repository.CreatedDate)}");

            var expectedRepositoryValidationException = new RepositoryValidationException(
                message: "Repository validation error occurred, fix errors and try again.",
                innerException: invalidRepositoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(RepositoryId))
                    .ReturnsAsync(storedRepository);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(invalidRepository.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedRepositoryValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Repository randomRepository = CreateRandomModifyRepository(randomDateTimeOffset);
            Repository invalidRepository = randomRepository;

            Repository storageRepository = randomRepository.DeepClone();
            invalidRepository.UpdatedDate = storageRepository.UpdatedDate;

            var invalidRepositoryException = new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values: $"Date is the same as {nameof(Repository.UpdatedDate)}");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(invalidRepository.Id))
                .ReturnsAsync(storageRepository);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
               await Assert.ThrowsAsync<RepositoryValidationException>(
                   testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryValidationException.Should().BeEquivalentTo(
                expectedRepositoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(invalidRepository.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}