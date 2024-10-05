// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveRepositoryByIdAsync()
        {
            // given
            Repository randomRepository = CreateRandomRepository();
            Repository storageRepository = randomRepository;
            Repository expectedRepository = storageRepository.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(randomRepository.Id))
                    .ReturnsAsync(storageRepository);

            // when
            Repository actualRepository =
                await this.repositoryService.RetrieveRepositoryByIdAsync(
                    randomRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(randomRepository.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}