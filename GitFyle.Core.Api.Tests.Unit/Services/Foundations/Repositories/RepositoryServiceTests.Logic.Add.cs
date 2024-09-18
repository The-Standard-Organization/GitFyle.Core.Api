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
        public async Task ShouldAddRespositoryAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Repository randomRepository = CreateRandomRepository(now);
            Repository inputRepository = randomRepository;
            Repository insertedRepository = inputRepository.DeepClone();
            Repository expectedRepository = insertedRepository.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertRepositoryAsync(inputRepository))
                    .ReturnsAsync(insertedRepository);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            Repository actualRepository =
                await this.repositoryService.AddRepositoryAsync(inputRepository);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertRepositoryAsync(inputRepository),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
