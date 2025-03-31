// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
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
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationExceptionOccursAsync(
                Xeption validationException)
        {
            // given
            Guid someContributionId = Guid.NewGuid();

            BadRequestObjectResult expectedInternalServerErrorObjectResult  =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedInternalServerErrorObjectResult );

            this.contributionServiceMock.Setup(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.DeleteContributionByIdAsync(someContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnDeleteIfReferenceExceptionOccursAsync()
        {
            // given
            Guid someContributionId = Guid.NewGuid();
            Contribution someContribution = CreateRandomContribution();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var invalidReferenceContributionException = 
                    new InvalidReferenceContributionException(
                        message: someMessage,
                        innerException: someInnerException,
                        data: someInnerException.Data);

            var contributionDependencyValidationException = 
                    new ContributionDependencyValidationException(
                        message: someMessage,
                        innerException: invalidReferenceContributionException,
                        data: invalidReferenceContributionException.Data);

            FailedDependencyObjectResult expectedFailedDependencyObjectResult = 
                    FailedDependency(invalidReferenceContributionException);

            var expectedActionResult = 
                    new ActionResult<Contribution>(expectedFailedDependencyObjectResult);


            this.contributionServiceMock.Setup(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionDependencyValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.DeleteContributionByIdAsync(someContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
                Xeption serverException)
        {
            // given
            Guid someContributionId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult  = 
                    InternalServerError(serverException);

            var expectedActionResult = 
                    new ActionResult<Contribution>(expectedInternalServerErrorObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.DeleteContributionByIdAsync(someContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfNotFoundContributionExceptionOccursAsync()
        {
            // given
            Guid someContributionId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundContributionException =
                new NotFoundContributionException(
                    message: someMessage);

            var contributionValidationException =
                new ContributionValidationException(
                    message: someMessage,
                    innerException: notFoundContributionException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundContributionException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedNotFoundObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.DeleteContributionByIdAsync(someContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfLockedContributionExceptionOccursAsync()
        {
            // given
            Guid someContributionId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var lockedContributionException =
                new LockedContributionException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: someMessage,
                    innerException: lockedContributionException,
                    data: someDictionaryData);

            LockedObjectResult expectedFailedDependencyObjectResult  =
                Locked(lockedContributionException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedFailedDependencyObjectResult );

            this.contributionServiceMock.Setup(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionDependencyValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.DeleteContributionByIdAsync(someContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
