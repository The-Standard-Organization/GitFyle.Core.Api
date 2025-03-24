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
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Contribution someContribution = CreateRandomContribution();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedBadRequestObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.AddContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PostContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.AddContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Contribution someContribution = CreateRandomContribution();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedInternalServerErrorObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.AddContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PostContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.AddContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsContributionErrorOccurredAsync()
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
                service.AddContributionAsync(It.IsAny<Contribution>()))
                    .ThrowsAsync(contributionDependencyValidationException);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PostContributionAsync(someContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.AddContributionAsync(It.IsAny<Contribution>()),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
