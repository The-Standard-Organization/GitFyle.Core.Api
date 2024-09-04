// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldAddConfigurationAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Configuration randomConfiguration =
                CreateRandomConfiguration(randomDate);

            Configuration inputConfiguration = randomConfiguration;
            Configuration expectedConfiguration = inputConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertConfigurationAsync(inputConfiguration))
                    .ReturnsAsync(expectedConfiguration);

            //when
            Configuration actualConfiguration =
                await this.configurationService.AddConfigurationAsync(inputConfiguration);

            //then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(
                    inputConfiguration),
                        Times.Once());

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
