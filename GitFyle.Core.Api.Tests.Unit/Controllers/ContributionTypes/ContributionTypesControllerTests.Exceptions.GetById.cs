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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccursAsync(
                Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.GetContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccursAsync(
                Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<ContributionType>(expectedBadRequestObjectResult);

            this.contributionTypeServiceMock.Setup(service =>
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.GetContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
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
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(contributionTypeValidationException);

            // when
            ActionResult<ContributionType> actualActionResult =
                await this.contributionTypesController.GetContributionTypeByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.contributionTypeServiceMock.Verify(service =>
                service.RetrieveContributionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.contributionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
