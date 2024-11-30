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
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Repository someRepository = CreateRandomRepository();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedBadRequestObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.PostRepositoryAsync(someRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Repository someRepository = CreateRandomRepository();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedInternalServerErrorObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.PostRepositoryAsync(someRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsRepositoryErrorOccurredAsync()
        {
            // given
            Repository someRepository = CreateRandomRepository();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var alreadyExistsRepositoryException =
                new AlreadyExistsRepositoryException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var repositoryDependencyValidationException =
                new RepositoryDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsRepositoryException,
                    data: someDictionaryData);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsRepositoryException);

            var expectedActionResult =
                new ActionResult<Repository>(expectedConflictObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()))
                    .ThrowsAsync(repositoryDependencyValidationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.PostRepositoryAsync(someRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.AddRepositoryAsync(It.IsAny<Repository>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
