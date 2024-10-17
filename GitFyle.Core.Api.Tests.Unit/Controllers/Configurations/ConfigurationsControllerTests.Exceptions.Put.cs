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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PutConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PutConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();
            string someMessage = GetRandomString();

            var notFoundConfigurationException =
                new NotFoundConfigurationException(
                    message: someMessage);

            var configurationValidationException =
                new ConfigurationValidationException(
                    message: someMessage,
                    innerException: notFoundConfigurationException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundConfigurationException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedNotFoundObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(configurationValidationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PutConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsConfigurationErrorOccursAsync()
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
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()))
                    .ThrowsAsync(configurationDependencyValidationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.PutConfigurationAsync(someConfiguration);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.ModifyConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
