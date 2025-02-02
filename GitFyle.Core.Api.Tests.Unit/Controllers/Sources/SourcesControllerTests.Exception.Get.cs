// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests
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
                new ActionResult<IQueryable<Source>>(
                    expectedInternalServerErrorObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.RetrieveAllSourcesAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Source>> actualActionResult =
                await this.sourcesController.GetAllSourcesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.RetrieveAllSourcesAsync(),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
