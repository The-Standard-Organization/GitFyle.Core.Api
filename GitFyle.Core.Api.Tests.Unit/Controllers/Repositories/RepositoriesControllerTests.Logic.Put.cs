// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Repository randomRepository = CreateRandomRepository();
            Repository inputRepository = randomRepository;
            Repository storageRepository = inputRepository.DeepClone();
            Repository expectedRepository = storageRepository.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedRepository);

            var expectedActionResult =
                new ActionResult<Repository>(expectedObjectResult);

            repositoryServiceMock
                .Setup(service => service.ModifyRepositoryAsync(inputRepository))
                    .ReturnsAsync(storageRepository);

            // when
            ActionResult<Repository> actualActionResult =
                await repositoriesController.PutRepositoryAsync(randomRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            repositoryServiceMock
               .Verify(service => service.ModifyRepositoryAsync(inputRepository),
                   Times.Once);

            repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
