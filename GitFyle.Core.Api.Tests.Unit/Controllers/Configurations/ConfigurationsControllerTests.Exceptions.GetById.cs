using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.RetrieveConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.GetConfigurationByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RetrieveConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
