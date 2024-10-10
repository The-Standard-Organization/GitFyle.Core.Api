using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xeptions;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.AddConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PostConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.AddConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedInternalServerErrorObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.AddConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PostConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.AddConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
