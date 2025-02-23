// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using GitFyle.Core.Api.Tests.Acceptance.Models.Configurations;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Configurations
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ConfigurationsApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public ConfigurationsApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;

        private static IQueryable<Configuration> CreateRandomConfigurations()
        {
            return CreateConfigurationFiller(DateTimeOffset.UtcNow)
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private async ValueTask<Configuration> PostRandomConfigurationAsync()
        {
            Configuration randomConfiguration = CreateRandomConfiguration(DateTimeOffset.UtcNow);
            await this.gitFyleCoreApiBroker.PostConfigurationAsync(randomConfiguration);

            return randomConfiguration;
        }

        private static Configuration UpdateConfigurationRandom(Configuration configuration)
        {
            var now = DateTimeOffset.UtcNow;
            configuration.UpdatedDate = now;

            return configuration;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

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
