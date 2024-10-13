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
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordsOnGetAsync()
        {
            // given
            IQueryable<Configuration> randomConfigurations = CreateRandomConfigurations();
            IQueryable<Configuration> storageConfigurations = randomConfigurations.DeepClone();
            IQueryable<Configuration> expectedConfiguration = storageConfigurations.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConfiguration);

            var expectedActionResult =
                new ActionResult<IQueryable<Configuration>>(expectedObjectResult);

            configurationServiceMock
            .Setup(service => service.RetrieveAllConfigurationsAsync())
                    .ReturnsAsync(storageConfigurations);

            // when
            ActionResult<IQueryable<Configuration>> actualActionResult = 
                await configurationsController.GetConfigurationsAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            configurationServiceMock
               .Verify(service => service.RetrieveAllConfigurationsAsync(),
                   Times.Once);

            configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
