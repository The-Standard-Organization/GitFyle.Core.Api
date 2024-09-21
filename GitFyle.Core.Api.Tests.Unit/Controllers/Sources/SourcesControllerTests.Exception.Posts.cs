// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Source someSource = CreateRandomSource();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Source>(expectedBadRequestObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.AddSourceAsync(It.IsAny<Source>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.PostSourceAsync(someSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.AddSourceAsync(It.IsAny<Source>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
