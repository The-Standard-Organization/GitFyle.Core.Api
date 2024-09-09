// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;
using System.Threading.Tasks;
using System;
using Force.DeepCloner;
using FluentAssertions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldModifySourceAsync()
        {
            //given
            int randomInPastMinute = GetRandomNegativeNumber();
            DateTimeOffset randomDate = GetRandomDateTimeOffset();

            Source randomSource = CreateRandomModifySource(
                randomDate.AddMinutes(randomInPastMinute));

            Source inputSource = randomSource;
            inputSource.UpdatedDate = randomDate;
            Source storageSource = inputSource.DeepClone();
            Source updatedSource = inputSource;
            Source expectedSource = updatedSource.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(inputSource.Id))
                    .ReturnsAsync(storageSource);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSourceAsync(inputSource))
                    .ReturnsAsync(updatedSource);

            //when
            Source actualSource =
                await this.sourceService.ModifySourceAsync(inputSource);

            //then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(inputSource.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSourceAsync(inputSource),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
