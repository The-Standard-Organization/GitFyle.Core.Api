// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Repositories
{
    public partial class RepositoriesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedBadRequestObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.DeleteRepositoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedBadRequestObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.DeleteRepositoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundRepositoryException =
                new NotFoundRepositoryException(
                    message: someMessage);

            var repositoryValidationException =
                new RepositoryValidationException(
                    message: someMessage,
                    innerException: notFoundRepositoryException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundRepositoryException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedNotFoundObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(repositoryValidationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.DeleteRepositoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var lockedRepositoryException =
                new LockedRepositoryException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var repositoryDependencyValidationException =
                new RepositoryDependencyValidationException(
                    message: someMessage,
                    innerException: lockedRepositoryException,
                    data: someDictionaryData);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedRepositoryException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedConflictObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(repositoryDependencyValidationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.DeleteRepositoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
