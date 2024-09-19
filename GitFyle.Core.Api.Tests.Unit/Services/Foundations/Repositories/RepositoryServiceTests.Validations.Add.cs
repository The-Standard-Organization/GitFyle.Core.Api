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
        public async Task ShouldThrowValidationExceptionOnAddIfRepositoryIsNullAndLogItAsync()
        {
            // given
            Repository nullRepository = null;

            var nullRepositoryException =
                new NullRepositoryException(
                    message: "Repository is null");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: nullRepositoryException);

            // when
            ValueTask<Repository> addRepositoryTask =
                this.repositoryService.AddRepositoryAsync(nullRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    addRepositoryTask.AsTask);

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
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfRepositoryIsInvalidAndLogItAsync(string invalidString)
        {
            //given
            var invalidRepository = new Repository
            {
                Id = Guid.Empty,
                Name = invalidString,
                Owner = invalidString,
                ExternalId = invalidString,
                SourceId = Guid.Empty,
                IsOrganization = default,
                IsPrivate = default,
                Token = invalidString,
                TokenExpireAt = default,
                Description = invalidString,
                ExternalCreatedAt = default,
                ExternalUpdatedAt = default,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default
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
                key: nameof(Repository.IsOrganization),
                values: "Value is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.IsPrivate),
                values: "Value is invalid");

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
                key: nameof(Repository.CreatedDate),
                values: "Date is invalid");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedBy),
                values: "Text is required");

            invalidRepositoryException.AddData(
                key: nameof(Repository.UpdatedDate),
                values: "Date is invalid");

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            // when
            ValueTask<Repository> addRepositoryTask =
                this.repositoryService.AddRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    addRepositoryTask.AsTask);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfRepositoryHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidRepository = CreateRandomRepository(randomDateTimeOffset);
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

            var expectedRepositoryValidationException =
                new RepositoryValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: invalidRepositoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> addRepositoryTask =
                this.repositoryService.AddRepositoryAsync(invalidRepository);

            RepositoryValidationException actualRepositoryValidationException =
                await Assert.ThrowsAsync<RepositoryValidationException>(
                    addRepositoryTask.AsTask);

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
    }
}