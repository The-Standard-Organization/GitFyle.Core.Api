// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Configurations
{
    public partial class ConfigurationsControllerTests : RESTFulController
    {
        private readonly Mock<IConfigurationService> configurationServiceMock;
        private readonly ConfigurationsController configurationsController;

        public ConfigurationsControllerTests()
        {
            this.configurationServiceMock = new Mock<IConfigurationService>();

            this.configurationsController =
                new ConfigurationsController(
                    configurationService: this.configurationServiceMock.Object);
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ConfigurationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConfigurationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ConfigurationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ConfigurationDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static IQueryable<Configuration> CreateRandomConfigurations() =>
            CreateConfigurationFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Configuration CreateRandomConfiguration() =>
            CreateConfigurationFiller().Create();

        private static Filler<Configuration> CreateConfigurationFiller()
        {
            var filler = new Filler<Configuration>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset())
                .OnProperty(configuration => configuration.CreatedBy).Use(user)
                .OnProperty(configuration => configuration.UpdatedBy).Use(user);

            return filler;
        }
    }
}
