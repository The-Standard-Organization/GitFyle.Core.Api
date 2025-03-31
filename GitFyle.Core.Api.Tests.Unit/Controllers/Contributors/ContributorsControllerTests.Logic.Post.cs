// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributors
{
    public partial class ContributorsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Contributor randomContributor = CreateRandomContributor();
            Contributor inputContributor = randomContributor;
            Contributor addedContributor = inputContributor;
            Contributor expectedContributor = addedContributor.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedContributor);

            var expectedActionResult =
                new ActionResult<Contributor>(expectedObjectResult);

            this.contributorServiceMock.Setup(service =>
                service.AddContributorAsync(inputContributor))
                    .ReturnsAsync(addedContributor);

            // when
            ActionResult<Contributor> actualActionResult =
                await this.contributorsController.PostContributorAsync(
                    inputContributor);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.contributorServiceMock.Verify(service =>
                service.AddContributorAsync(inputContributor),
                    Times.Once);

            this.contributorServiceMock.VerifyNoOtherCalls();
        }
    }
}
