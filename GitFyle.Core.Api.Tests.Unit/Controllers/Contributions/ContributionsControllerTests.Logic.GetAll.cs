// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnOkOnGetAllContributionsAsync()
        {
            // given
            IQueryable<Contribution> randomContributions =
                CreateRandomnContributions();

            IQueryable<Contribution> retrievedContributions =
                randomContributions;

            IQueryable<Contribution> expectedContributions
                = retrievedContributions.DeepClone();

            OkObjectResult expectedObjectResult =
                Ok(retrievedContributions);

            var expectedActionResult =
                new ActionResult<IQueryable<Contribution>>(
                    expectedObjectResult);

            this.contributionServiceMock.Setup(service =>
                service.RetrieveAllContributionsAsync())
                    .ReturnsAsync(retrievedContributions);

            // when
            ActionResult<IQueryable<Contribution>> actualActionResult =
                await this.contributionsController.GetAllContributionsAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);
        }
    }
}
