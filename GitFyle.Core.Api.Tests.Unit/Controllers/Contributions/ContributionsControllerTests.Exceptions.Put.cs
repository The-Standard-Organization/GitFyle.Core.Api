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
    }
}
