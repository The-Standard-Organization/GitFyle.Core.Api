// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributions
{
    public partial class ContributionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetAllIfServerExceptionOccursAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Contribution>>(
                    expectedObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.RetrieveAllContributionsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Contribution>> actualActionResult =
                await this.contributionsController.GetAllContributionsAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RetrieveAllContributionsAsync(),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
