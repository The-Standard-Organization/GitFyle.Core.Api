// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Contribution someContribution = CreateRandomContribution();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedBadRequestObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PutContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnPutIfReferenceErrorOccursAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var invalidReferenceContributionException =
                new InvalidReferenceContributionException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: someMessage,
                    innerException: invalidReferenceContributionException,
                    data: someDictionaryData);

            FailedDependencyObjectResult expectedConflictObjectResult =
               FailedDependency(invalidReferenceContributionException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedConflictObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(contributionDependencyValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PutContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Contribution someContribution = CreateRandomContribution();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedBadRequestObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PutContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
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
                service.ModifyContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(contributionValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PutContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsContributionErrorOccursAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var alreadyExistsContributionException =
                new AlreadyExistsContributionException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsContributionException,
                    data: someDictionaryData);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsContributionException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedConflictObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(contributionDependencyValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PutContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.ModifyContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
