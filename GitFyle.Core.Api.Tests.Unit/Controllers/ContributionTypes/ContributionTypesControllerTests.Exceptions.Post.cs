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
            ContributionType someContributionType = CreateRandomContributionType();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerExceptionOccurredAsync(
                Xeption serverException)
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedInternalServerErrorObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsContributionTypeExceptionOccurredAsync()
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            var alreadyExistsContributionTypeException =
                    new AlreadyExistsContributionTypeException(
                        message: someMessage,
                        innerException: someInnerException,
                        data: someInnerException.Data);

            var contributionTypeDependencyValidationException =
                    new ContributionTypeDependencyValidationException(
                        message: someMessage,
                        innerException: alreadyExistsContributionTypeException,
                        data: someDictionaryData);

            ConflictObjectResult expectedConflictObjectResult =
                    Conflict(alreadyExistsContributionTypeException);

            var expectedActionResult =
                     new ActionResult<ContributionType>(expectedConflictObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFailedDependencyOnPostIfReferenceExceptionOccursAsync()
        {
            // given
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
                    new ActionResult<ContributionType>(expectedFailedDependencyObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()))
                    .ThrowsAsync(contributionTypeDependencyValidationException);

            // when
            ActionResult<ContributionType> actualActionResult = 
                    await this.contributionTypesController.PostContributionTypeAsync(someContributionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.AddContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
