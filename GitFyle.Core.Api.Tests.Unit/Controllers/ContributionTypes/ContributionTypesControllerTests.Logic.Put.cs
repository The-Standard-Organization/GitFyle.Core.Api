// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            ContributionType randomContributionType = CreateRandomContributionType();
            ContributionType inputContributionType = randomContributionType;
            ContributionType storageContributionType = inputContributionType.DeepClone();
            ContributionType expectedContributionType = storageContributionType.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContributionType);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedObjectResult);

            contributionTypeServiceMock
                .Setup(service => service.ModifyContributionTypeAsync(inputContributionType))
                    .ReturnsAsync(storageContributionType);

            // when
            ActionResult<ContributionType> actualActionResult =
                await contributionTypesController.PutContributionTypeAsync(randomContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            contributionTypeServiceMock
               .Verify(service => service.ModifyContributionTypeAsync(inputContributionType),
                   Times.Once);

            contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
