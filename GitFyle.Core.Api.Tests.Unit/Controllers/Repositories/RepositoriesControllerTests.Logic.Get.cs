// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnOkWithRepositoriesOnGetAsync()
        {
            // given
            IQueryable<Repository> randomRepositories = CreateRandomRepositories();
            IQueryable<Repository> storageRepositories = randomRepositories.DeepClone();
            IQueryable<Repository> expectedRepository = storageRepositories.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedRepository);

            var expectedActionResult =
                new ActionResult<IQueryable<Repository>>(expectedObjectResult);

            repositoryServiceMock
            .Setup(service => service.RetrieveAllRepositoriesAsync())
                    .ReturnsAsync(storageRepositories);

            // when
            ActionResult<IQueryable<Repository>> actualActionResult =
                await repositoriesController.GetRepositoriesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            repositoryServiceMock
               .Verify(service => service.RetrieveAllRepositoriesAsync(),
                   Times.Once);

            repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
