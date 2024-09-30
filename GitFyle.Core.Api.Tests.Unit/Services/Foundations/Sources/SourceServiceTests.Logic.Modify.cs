// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldModifySourceAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            Source randomModifySource =
                CreateRandomModifySource(randomDateOffset);

            Source inputSource = randomModifySource.DeepClone();
            Source storageSource = randomModifySource.DeepClone();
            storageSource.UpdatedDate = storageSource.CreatedDate;
            Source updatedSource = inputSource.DeepClone();
            Source expectedSource = updatedSource.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(inputSource.Id))
                    .ReturnsAsync(storageSource);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSourceAsync(inputSource))
                    .ReturnsAsync(updatedSource);

            // when
            Source actualSource =
                await this.sourceService.ModifySourceAsync(inputSource);

            // then
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
