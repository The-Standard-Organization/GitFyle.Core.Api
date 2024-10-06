// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRepositoryByIdAsync()
        {
            // given
            Guid someRepositoryId = Guid.NewGuid();
            Repository randomRepository = CreateRandomRepository();
            Repository storageRepository = randomRepository;
            Repository inputRepository = storageRepository;
            Repository removedRepository = inputRepository;
            Repository expectedRepository = removedRepository.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(someRepositoryId))
                    .ReturnsAsync(storageRepository);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteRepositoryAsync(inputRepository))
                    .ReturnsAsync(removedRepository);

            // when
            Repository actualRepository =
                await this.repositoryService.RemoveRepositoryByIdAsync(someRepositoryId);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(someRepositoryId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteRepositoryAsync(storageRepository),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}