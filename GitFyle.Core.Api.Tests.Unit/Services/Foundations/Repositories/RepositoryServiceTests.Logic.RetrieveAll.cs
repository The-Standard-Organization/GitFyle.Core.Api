// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllRepositoriesAsync()
        {
            // given
            IQueryable<Repository> randomRepositories = CreateRandomRepositories();
            IQueryable<Repository> storageRepositories = randomRepositories;
            IQueryable<Repository> expectedRepositories = storageRepositories;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllRepositoriesAsync())
                    .ReturnsAsync(storageRepositories);

            // when
            IQueryable<Repository> actualRepositories =
                await this.repositoryService.RetrieveAllRepositoriesAsync();

            // then
            actualRepositories.Should().BeEquivalentTo(expectedRepositories);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllRepositoriesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}