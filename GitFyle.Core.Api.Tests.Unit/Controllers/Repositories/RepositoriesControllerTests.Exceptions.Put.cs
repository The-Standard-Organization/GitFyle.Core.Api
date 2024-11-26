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
        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Repository someRepository = CreateRandomRepository();
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
                service.ModifyRepositoryAsync(It.IsAny<Repository>()))
                    .ThrowsAsync(repositoryValidationException);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.PutRepositoryAsync(someRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.ModifyRepositoryAsync(It.IsAny<Repository>()),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
