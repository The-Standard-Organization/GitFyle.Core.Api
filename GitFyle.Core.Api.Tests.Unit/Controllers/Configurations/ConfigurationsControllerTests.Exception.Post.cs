// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

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

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsConfigurationErrorOccurredAsync()
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsConfigurationException =
                new AlreadyExistsConfigurationException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var configurationDependencyValidationException =
                new ConfigurationDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsConfigurationException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsConfigurationException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedConflictObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.AddConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(configurationDependencyValidationException);

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
