// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Repositories
{
    public partial class RepositoriesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            Repository randomRepository = CreateRandomRepository();
            Guid inputId = randomRepository.Id;
            Repository storageRepository = randomRepository;
            Repository expectedRepository = storageRepository.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedRepository);

            var expectedActionResult =
                new ActionResult<Repository>(expectedObjectResult);

            this.repositoryServiceMock.Setup(service =>
                service.RetrieveRepositoryByIdAsync(inputId))
                    .ReturnsAsync(storageRepository);

            // when
            ActionResult<Repository> actualActionResult =
                await repositoriesController.GetRepositoryByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.repositoryServiceMock.Verify(service =>
                service.RetrieveRepositoryByIdAsync(inputId),
                    Times.Once());

            this.repositoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
