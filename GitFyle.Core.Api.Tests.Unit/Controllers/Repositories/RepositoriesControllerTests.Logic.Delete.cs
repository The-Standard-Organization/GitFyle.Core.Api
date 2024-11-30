// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Repositories
{
    public partial class RepositoriesControllerTests
    {
        [Fact]
        public async Task ShouldRemoveRepositoryOnDeleteByIdAsync()
        {
            // given
            Repository randomRepository = CreateRandomRepository();
            Guid inputId = randomRepository.Id;
            Repository storageRepository = randomRepository;
            Repository expectedRepository = storageRepository.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedRepository);

            var expectedActionResult =
                new ActionResult<Repository>(expectedObjectResult);

            repositoryServiceMock
                .Setup(service => service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageRepository);

            // when
            ActionResult<Repository> actualActionResult =
                await repositoriesController.DeleteRepositoryByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            repositoryServiceMock
                .Verify(service => service.RemoveRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
