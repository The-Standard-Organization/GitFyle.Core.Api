// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            Contribution inputContribution = randomContribution;
            Contribution storageContribution = inputContribution.DeepClone();
            Contribution expectedContribution = storageContribution.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContribution);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedObjectResult);

            contributionServiceMock
                .Setup(service => service.ModifyContributionAsync(inputContribution))
                    .ReturnsAsync(storageContribution);

            // when
            ActionResult<Contribution> actualActionResult =
                await contributionsController.PutContributionAsync(randomContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            contributionServiceMock
               .Verify(service => service.ModifyContributionAsync(inputContribution),
                   Times.Once);

            contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
