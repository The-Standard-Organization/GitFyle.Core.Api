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
        public async Task ShouldModifyRepositoryAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            Repository randomModifyRepository =
                CreateRandomModifyRepository(randomDateOffset);

            Repository inputRepository = randomModifyRepository.DeepClone();
            Repository storageRepository = randomModifyRepository.DeepClone();
            storageRepository.UpdatedDate = storageRepository.CreatedDate;
            Repository updatedRepository = inputRepository.DeepClone();
            Repository expectedRepository = updatedRepository.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(inputRepository.Id))
                    .ReturnsAsync(storageRepository);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateRepositoryAsync(inputRepository))
                    .ReturnsAsync(updatedRepository);

            // when
            Repository actualRepository =
                await this.repositoryService.ModifyRepositoryAsync(inputRepository);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(inputRepository.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateRepositoryAsync(inputRepository),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
