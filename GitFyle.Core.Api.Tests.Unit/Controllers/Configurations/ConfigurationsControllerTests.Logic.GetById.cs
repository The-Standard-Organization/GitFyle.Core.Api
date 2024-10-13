// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            Configuration randomConfiguration = CreateRandomConfiguration();
            Guid inputId = randomConfiguration.Id;
            Configuration storageConfiguration = randomConfiguration;
            Configuration expectedConfiguration = storageConfiguration.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConfiguration);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.RetrieveConfigurationByIdAsync(inputId))
                    .ReturnsAsync(storageConfiguration);
            // when
            ActionResult<Configuration> actualActionResult =
                await configurationsController.GetConfigurationByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RetrieveConfigurationByIdAsync(inputId),
                    Times.Once());

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
