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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogitAsync()
        {
            // given
            Guid someContributionTypeId = Guid.Empty;

            var invalidContributionTypeException =
                new InvalidContributionTypeException(
                    message: "ContributionType is invalid, fix the errors and try again.");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.Id),
                values: "Id is invalid");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: invalidContributionTypeException);

            // when
            ValueTask<ContributionType> removeContributionTypeByIdTask =
                this.contributionTypeService.RemoveContributionTypeByIdAsync(someContributionTypeId);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    removeContributionTypeByIdTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}