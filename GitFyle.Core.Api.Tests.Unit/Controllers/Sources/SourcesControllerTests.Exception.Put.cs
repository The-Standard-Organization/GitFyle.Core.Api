// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
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
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Source someSource = CreateRandomSource();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Source>(expectedBadRequestObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.ModifySourceAsync(It.IsAny<Source>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.PutSourceAsync(someSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.ModifySourceAsync(It.IsAny<Source>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Source someSource = CreateRandomSource();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Source>(expectedBadRequestObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.ModifySourceAsync(It.IsAny<Source>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.PutSourceAsync(someSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.ModifySourceAsync(It.IsAny<Source>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Source someSource = CreateRandomSource();
            string someMessage = GetRandomString();

            var notFoundSourceException =
                new NotFoundSourceException(
                    message: someMessage);

            var sourceValidationException =
                new SourceValidationException(
                    message: someMessage,
                    innerException: notFoundSourceException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundSourceException);

            var expectedActionResult =
                new ActionResult<Source>(expectedNotFoundObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.ModifySourceAsync(It.IsAny<Source>()))
                    .ThrowsAsync(sourceValidationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.PutSourceAsync(someSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.ModifySourceAsync(It.IsAny<Source>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
