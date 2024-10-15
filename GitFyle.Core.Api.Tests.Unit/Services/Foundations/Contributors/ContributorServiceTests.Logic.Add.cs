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
        public async Task ShouldAddContributorAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Contributor randomContributor = CreateRandomContributor(dateTimeOffset: now);
            Contributor inputContributor = randomContributor;
            Contributor insertedContributor = inputContributor.DeepClone();
            Contributor expectedContributor = insertedContributor.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertContributorAsync(inputContributor))
                    .ReturnsAsync(insertedContributor);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            Contributor actualContributor =
                await this.contributorService.AddContributorAsync(inputContributor);

            // then
            actualContributor.Should().BeEquivalentTo(expectedContributor);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(inputContributor),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
