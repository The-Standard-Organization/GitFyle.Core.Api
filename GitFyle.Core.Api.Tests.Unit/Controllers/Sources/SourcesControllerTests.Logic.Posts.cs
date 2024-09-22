// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source inputSource = randomSource;
            Source addedSource = inputSource;
            Source expectedSource = addedSource.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedSource);

            var expectedActionResult =
                new ActionResult<Source>(expectedObjectResult);

            this.sourceServiceMock.Setup(service =>
                service.AddSourceAsync(inputSource))
                    .ReturnsAsync(addedSource);

            // when
            ActionResult<Source> actualActionResult =
                await this.sourcesController.PostSourceAsync(
                    inputSource);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.sourceServiceMock.Verify(service =>
                service.AddSourceAsync(inputSource),
                    Times.Once);

            this.sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
