// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests
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
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundContributionTypeException =
                new NotFoundContributionTypeException(
                    message: someMessage);

            var contributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: someMessage,
                    innerException: notFoundContributionTypeException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundContributionTypeException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedNotFoundObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var lockedContributionTypeException =
                new LockedContributionTypeException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributionTypeDependencyValidationException =
                new ContributionTypeDependencyValidationException(
                    message: someMessage,
                    innerException: lockedContributionTypeException,
                    data: someDictionaryData);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedContributionTypeException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedConflictObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnDeleteIfReferenceErrorOccursAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            ContributionType someContributionType = CreateRandomContributionType();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var invalidReferenceContributionTypeException =
                new InvalidReferenceContributionTypeException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var contributionTypeDependencyValidationException =
                new ContributionTypeDependencyValidationException(
                    message: someMessage,
                    innerException: invalidReferenceContributionTypeException,
                    data: invalidReferenceContributionTypeException.Data);

            FailedDependencyObjectResult expectedConflictObjectResult =
               FailedDependency(invalidReferenceContributionTypeException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedConflictObjectResult);


            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
