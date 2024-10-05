// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Guid inputId = randomSource.Id;
            Source storageSource = randomSource;
            Source expectedSource = storageSource.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedSource);

            var expectedActionResult =
                new ActionResult<Source>(expectedObjectResult);

            sourceServiceMock
                .Setup(service => service.RetrieveSourceByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageSource);

            // when
            ActionResult<Source> actualActionResult = await sourcesController.GetSourceByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            sourceServiceMock
                .Verify(service => service.RetrieveSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            sourceServiceMock.VerifyNoOtherCalls();
        }
    }
}
