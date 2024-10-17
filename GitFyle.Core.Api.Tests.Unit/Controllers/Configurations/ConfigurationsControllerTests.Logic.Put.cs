// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Configuration randomConfiguration = CreateRandomConfiguration();
            Configuration inputConfiguration = randomConfiguration;
            Configuration storageConfiguration = inputConfiguration.DeepClone();
            Configuration expectedConfiguration = storageConfiguration.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConfiguration);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedObjectResult);

            configurationServiceMock
                .Setup(service => service.ModifyConfigurationAsync(inputConfiguration))
                    .ReturnsAsync(storageConfiguration);

            // when
            ActionResult<Configuration> actualActionResult = 
                await configurationsController.PutConfigurationAsync(randomConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            configurationServiceMock
               .Verify(service => service.ModifyConfigurationAsync(inputConfiguration),
                   Times.Once);

            configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
