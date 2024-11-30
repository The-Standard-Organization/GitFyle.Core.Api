// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Repositories
{
    public partial class RepositoriesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Repository randomRepository = CreateRandomRepository();
            Repository inputRepository = randomRepository;
            Repository addedRepository = inputRepository;
            Repository expectedRepository = addedRepository.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedRepository);

            var expectedActionResult =
                new ActionResult<Repository>(expectedObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.AddRepositoryAsync(inputRepository))
                    .ReturnsAsync(addedRepository);

            // when
            ActionResult<Repository> actualActionResult =
                await this.repositoriesController.PostRepositoryAsync(
                    inputRepository);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.AddRepositoryAsync(inputRepository),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
