// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source inputSource = randomSource;
            Source storageSource = inputSource.DeepClone();
            Source expectedSource = storageSource.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSource);

            var expectedActionResult =
                new ActionResult<Source>(expectedObjectResult);

            sourceServiceMock
                .Setup(service => service.ModifySourceAsync(inputSource))
                    .ReturnsAsync(storageSource);

            // when
            ActionResult<Source> actualActionResult = await sourcesController.PutSourceAsync(randomSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            sourceServiceMock
               .Verify(service => service.ModifySourceAsync(inputSource),
                   Times.Once);

            sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
