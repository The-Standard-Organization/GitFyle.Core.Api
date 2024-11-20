// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldRemoveContributorByIdAsync()
        {
            // given
            Guid someContributorId = Guid.NewGuid();
            Contributor randomContributor = CreateRandomContributor();
            Contributor storageContributor = randomContributor;
            Contributor inputContributor = storageContributor;
            Contributor removedContributor = inputContributor;
            Contributor expectedContributor = removedContributor.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(someContributorId))
                    .ReturnsAsync(storageContributor);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteContributorAsync(inputContributor))
                    .ReturnsAsync(removedContributor);

            // when
            Contributor actualContributor =
                await this.contributorService.RemoveContributorByIdAsync(someContributorId);

            // then
            actualContributor.Should().BeEquivalentTo(expectedContributor);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(someContributorId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteContributorAsync(storageContributor),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}