// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributions
{
    public partial class ContributionsControllerTests
    {
        [Fact]
        public async Task ShouldRemoveContributionOnDeleteByIdAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            Guid inputContributionId = randomContribution.Id;
            Contribution storageContribution = randomContribution;
            Contribution expectedContribution = storageContribution.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContribution);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedObjectResult);

            contributionServiceMock
                .Setup(service => service.RemoveContributionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageContribution);

            // when
            ActionResult<Contribution> actualActionResult =
                await contributionsController.DeleteContributionByIdAsync(inputContributionId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            contributionServiceMock
                .Verify(service => service.RemoveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
