// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Configurations
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ConfigurationsApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public ConfigurationsApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;

        private static Configuration CreateRandomConfiguration(DateTimeOffset dateTimeOffset) =>
            CreateConfigurationFiller(dateTimeOffset).Create();

        private static Filler<Configuration> CreateConfigurationFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<Configuration>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(configuration => configuration.CreatedBy).Use(user)
                .OnProperty(configuration => configuration.UpdatedBy).Use(user);

            return filler;
        }
    }
}
