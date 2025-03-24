// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.repositoriesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedInternalServerErrorObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.repositoriesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
