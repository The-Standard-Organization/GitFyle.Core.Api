// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.DeleteConfigurationByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedBadRequestObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.DeleteConfigurationByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(configurationValidationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.DeleteConfigurationByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedConfigurationException =
                new LockedConfigurationException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var configurationDependencyValidationException =
                new ConfigurationDependencyValidationException(
                    message: someMessage,
                    innerException: lockedConfigurationException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedConfigurationException);

            var expectedActionResult =
                new ActionResult<Configuration>(expectedConflictObjectResult);

            this.configurationServiceMock.Setup(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(configurationDependencyValidationException);

            // when
            ActionResult<Configuration> actualActionResult =
                await this.configurationsController.DeleteConfigurationByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.configurationServiceMock.Verify(service =>
                service.RemoveConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.configurationServiceMock.VerifyNoOtherCalls();
        }
    }
}
