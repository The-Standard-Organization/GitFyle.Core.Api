// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionTypeIsNullAndLogItAsync()
        {
            // given
            ContributionType nullContributionType = null;

            var nullContributionTypeException =
                new NullContributionTypeException(
                    message: "ContributionType is null");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: nullContributionTypeException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(nullContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
