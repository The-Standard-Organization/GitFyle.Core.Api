using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using Moq;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests
    {
        private readonly Mock<IConfigurationService> configurationServiceMock;
        private readonly ConfigurationsController configurationsController;

        public ConfigurationsControllerTests()
        {
            this.configurationServiceMock = new Mock<IConfigurationService>();

            this.configurationsController = 
                new ConfigurationsController(
                    configurationService:  this.configurationServiceMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Configuration CreateRandomConfiguration() =>
            CreateConfigurationFiller().Create();

        private static Filler<Configuration> CreateConfigurationFiller()
        {
            var filler = new Filler<Configuration>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
