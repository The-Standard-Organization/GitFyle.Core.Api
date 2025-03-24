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
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            Guid inputId = randomContribution.Id;
            Contribution storageContribution = randomContribution;
            Contribution expectedContribution = storageContribution.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContribution);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedObjectResult);

            contributionServiceMock
                .Setup(service => service.RetrieveContributionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageContribution);

            // when
            ActionResult<Contribution> actualActionResult = await contributionsController.GetContributionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            contributionServiceMock
                .Verify(service => service.RetrieveContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
