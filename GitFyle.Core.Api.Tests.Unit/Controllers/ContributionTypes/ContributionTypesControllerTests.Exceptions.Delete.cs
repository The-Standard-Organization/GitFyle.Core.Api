// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationExceptionOccursAsync(
            Xeption validationException)
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerExceptionOccurredAsync(
            Xeption serverException)
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedInternalServerErrorObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfContributionTypeDoesNotExistAsync()
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();
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
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfLockedContributionTypeExceptionOccursAsync()
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();
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

            LockedObjectResult expectedLockedObjectResult =
                    Locked(lockedContributionTypeException);

            var expectedActionResult =
                    new ActionResult<ContributionType>(expectedLockedObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnDeleteIfReferenceExceptionOccursAsync()
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();
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

            FailedDependencyObjectResult expectedFailedDependencyObjectResult = 
                    FailedDependency(invalidReferenceContributionTypeException);

            var expectedActionResult = 
                    new ActionResult<ContributionType>(expectedFailedDependencyObjectResult );


            this.contributionTypeServiceMock.Setup(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.DeleteContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RemoveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
