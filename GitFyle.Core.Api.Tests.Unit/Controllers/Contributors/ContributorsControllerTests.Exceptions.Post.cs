// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributors
{
    public partial class ContributorsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            Contributor someContributor = CreateRandomContributor();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Contributor>(expectedBadRequestObjectResult);

            this.contributorServiceMock.Setup(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Contributor> actualActionResult =
                await this.contributorsController.PostContributorAsync(someContributor);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributorServiceMock.Verify(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()),
                    Times.Once);

            this.contributorServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Contributor someContributor = CreateRandomContributor();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Contributor>(expectedInternalServerErrorObjectResult);

            this.contributorServiceMock.Setup(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Contributor> actualActionResult =
                await this.contributorsController.PostContributorAsync(someContributor);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributorServiceMock.Verify(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()),
                    Times.Once);

            this.contributorServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsContributorErrorOccurredAsync()
        {
            // given
            Contributor someContributor = CreateRandomContributor();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var alreadyExistsContributorException =
                new AlreadyExistsContributorException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributorDependencyValidationException =
                new ContributorDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsContributorException,
                    data: someDictionaryData);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsContributorException);

            var expectedActionResult =
                new ActionResult<Contributor>(expectedConflictObjectResult);

            this.contributorServiceMock.Setup(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()))
                    .ThrowsAsync(contributorDependencyValidationException);

            // when
            ActionResult<Contributor> actualActionResult =
                await this.contributorsController.PostContributorAsync(someContributor);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributorServiceMock.Verify(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()),
                    Times.Once);

            this.contributorServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnPostIfReferenceExceptionOccursAsync()
        {
            // given
            Contributor someContributor = CreateRandomContributor();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var invalidReferenceContributorException =
                new InvalidReferenceContributorException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributorDependencyValidationException =
                new ContributorDependencyValidationException(
                    message: someMessage,
                    innerException: invalidReferenceContributorException,
                    data: invalidReferenceContributorException.Data);

            FailedDependencyObjectResult expectedFailedDependencyObjectResult =
                    FailedDependency(invalidReferenceContributorException);

            var expectedActionResult =
                new ActionResult<Contributor>(expectedFailedDependencyObjectResult);

            this.contributorServiceMock.Setup(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()))
                    .ThrowsAsync(contributorDependencyValidationException);

            // when
            ActionResult<Contributor> actualActionResult =
                await this.contributorsController.PostContributorAsync(someContributor);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributorServiceMock.Verify(service =>
                service.AddContributorAsync(It.IsAny<Contributor>()),
                    Times.Once);

            this.contributorServiceMock.VerifyNoOtherCalls();
        }
    }
}
