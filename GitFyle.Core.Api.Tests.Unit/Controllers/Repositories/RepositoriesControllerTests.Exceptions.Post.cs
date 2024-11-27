using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
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
    }
}
