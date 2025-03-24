// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnOkWithContributionTypesOnGetAsync()
        {
            // given
            IQueryable<ContributionType> randomContributionTypes = CreateRandomContributionTypes();
            IQueryable<ContributionType> storageContributionTypes = randomContributionTypes.DeepClone();
            IQueryable<ContributionType> expectedContributionType = storageContributionTypes.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContributionType);

            var expectedActionResult =
                new ActionResult<IQueryable<ContributionType>>(expectedObjectResult);

            contributionTypeServiceMock
            .Setup(service => service.RetrieveAllContributionTypesAsync())
                    .ReturnsAsync(storageContributionTypes);

            // when
            ActionResult<IQueryable<ContributionType>> actualActionResult =
                await repositoriesController.GetContributionTypesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            contributionTypeServiceMock
               .Verify(service => service.RetrieveAllContributionTypesAsync(),
                   Times.Once);

            contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
