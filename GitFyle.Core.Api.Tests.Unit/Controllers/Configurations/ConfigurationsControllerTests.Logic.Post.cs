// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
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
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Configuration randomConfiguration = CreateRandomConfiguration();
            Configuration inputConfiguration = randomConfiguration;
            Configuration addedConfiguration = inputConfiguration;
            Configuration expectedConfiguration = addedConfiguration.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedConfiguration);

            var expectedActionResult = 
                new ActionResult<Configuration>(expectedObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.AddConfigurationAsync(inputConfiguration))
                    .ReturnsAsync(addedConfiguration);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PostConfigurationAsync(
                    inputConfiguration);
                
            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.configurationServiceMock.Verify(service => 
                service.AddConfigurationAsync(inputConfiguration), 
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
