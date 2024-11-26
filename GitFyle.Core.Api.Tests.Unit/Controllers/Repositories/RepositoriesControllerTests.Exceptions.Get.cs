// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
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
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Repository>>(expectedInternalServerErrorObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RetrieveAllRepositoriesAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Repository>> actualActionResult =
                await this.repositoriesController.GetRepositoriesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RetrieveAllRepositoriesAsync(),
                    Times.Once);

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
