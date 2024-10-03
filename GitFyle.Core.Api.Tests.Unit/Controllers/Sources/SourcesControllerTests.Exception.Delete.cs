// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Source>(expectedBadRequestObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.DeleteSourceByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Source>(expectedBadRequestObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.DeleteSourceByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sourceValidationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.DeleteSourceByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedSourceException =
                new LockedSourceException(
                    message: someMessage,
                    innerException: someInnerException);

            var sourceDependencyValidationException =
                new SourceDependencyValidationException(
                    message: someMessage,
                    innerException: lockedSourceException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedSourceException);

            var expectedActionResult =
                new ActionResult<Source>(expectedConflictObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sourceDependencyValidationException);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.DeleteSourceByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.RemoveSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
