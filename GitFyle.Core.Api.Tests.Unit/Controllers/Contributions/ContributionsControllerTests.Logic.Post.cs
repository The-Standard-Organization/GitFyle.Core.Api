// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributions
{
    public partial class ContributionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            Contribution inputContribution = randomContribution;
            Contribution addedContribution = inputContribution;
            Contribution expectedContribution = addedContribution.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedContribution);

            var expectedActionResult =
                new ActionResult<Contribution>(expectedObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.AddContributionAsync(inputContribution))
                    .ReturnsAsync(addedContribution);

            // when
            ActionResult<Contribution> actualActionResult =
                await this.contributionsController.PostContributionAsync(
                    inputContribution);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.contributionServiceMock.Verify(service =>
                service.AddContributionAsync(inputContribution),
                    Times.Once);

            this.contributionServiceMock.VerifyNoOtherCalls();
        }
    }
}
