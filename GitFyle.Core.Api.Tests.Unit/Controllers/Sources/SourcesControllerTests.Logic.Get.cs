// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordsOnGetAsync()
        {
            // given
            IQueryable<Source> randomSources = CreateRandomSources();
            IQueryable<Source> storageSources = randomSources.DeepClone();
            IQueryable<Source> expectedSource = storageSources.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSource);

            var expectedActionResult =
                new ActionResult<IQueryable<Source>>(expectedObjectResult);

            sourceServiceMock
                .Setup(service => service.RetrieveAllSourcesAsync())
                    .ReturnsAsync(storageSources);

            // when
            ActionResult<IQueryable<Source>> actualActionResult = await sourcesController.GetAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            sourceServiceMock
               .Verify(service => service.RetrieveAllSourcesAsync(),
                   Times.Once);

            sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
