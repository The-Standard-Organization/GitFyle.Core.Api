// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

    }
}
