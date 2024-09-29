using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldModifyConfigurationAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Configuration randomModifyConfiguration =
                CreateRandomModifyConfiguration(randomDateTimeOffset);

            Configuration inputConfiguration = randomModifyConfiguration.DeepClone();
            Configuration storageConfiguration = randomModifyConfiguration.DeepClone();
            storageConfiguration.UpdatedDate = storageConfiguration.CreatedDate;
            Configuration updatedConfiguration = inputConfiguration.DeepClone();
            Configuration expectedConfiguration = updatedConfiguration.DeepClone();

            this.datetimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(inputConfiguration.Id))
                    .ReturnsAsync(storageConfiguration);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateConfigurationAsync(inputConfiguration))
                    .ReturnsAsync(updatedConfiguration);

            // when
            Configuration actualConfiguration =
                await this.configurationService.ModifyConfigurationAsync(inputConfiguration);

            // then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);

            this.datetimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffsetAsync(), 
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(inputConfiguration.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(inputConfiguration),
                    Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
