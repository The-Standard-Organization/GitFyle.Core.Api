// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            ContributionType randomContributionType = CreateRandomContributionType();
            ContributionType inputContributionType = randomContributionType;
            ContributionType addedContributionType = inputContributionType;
            ContributionType expectedContributionType = addedContributionType.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedContributionType);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(inputContributionType))
                    .ReturnsAsync(addedContributionType);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.repositoriesController.PostContributionTypeAsync(
                    inputContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(inputContributionType),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
