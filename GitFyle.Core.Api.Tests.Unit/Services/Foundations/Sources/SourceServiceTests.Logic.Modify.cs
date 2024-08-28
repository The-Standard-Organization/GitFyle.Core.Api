// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Source randomSource = CreateRandomSource(dateTimeOffset: randomDate);
            Source inputSource = randomSource;
            Source storageSource = inputSource.DeepClone();
            Source modifiedSource = inputSource;
            modifiedSource.UpdatedDate = randomDate.AddMinutes(1);
            Source expectedSource = modifiedSource.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectSourceByIdAsync(inputSource.Id))
                    .ReturnsAsync(storageSource);

            this.storageBrokerMock.Setup(broker => 
                broker.UpdateSourceAsync(modifiedSource))
                    .ReturnsAsync(expectedSource);

            // when
            Source actualSource = 
                await this.sourceService.ModifySourceAsync(modifiedSource);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()), 
                    Times.Once());

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateSourceAsync(It.IsAny<Source>()), 
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
