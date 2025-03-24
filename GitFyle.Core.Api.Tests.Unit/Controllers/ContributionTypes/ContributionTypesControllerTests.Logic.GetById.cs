// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            ContributionType randomContributionType = CreateRandomContributionType();
            Guid inputId = randomContributionType.Id;
            ContributionType storageContributionType = randomContributionType;
            ContributionType expectedContributionType = storageContributionType.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedContributionType);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RetrieveContributionTypeByIdAsync(inputId))
                    .ReturnsAsync(storageContributionType);

            // when
            ActionResult<ContributionType> actualActionResult =
                await contributionTypesController.GetContributionTypeByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RetrieveContributionTypeByIdAsync(inputId),
                    Times.Once());

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
