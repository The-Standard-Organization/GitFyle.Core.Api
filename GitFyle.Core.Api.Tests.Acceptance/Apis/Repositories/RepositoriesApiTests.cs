// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using GitFyle.Core.Api.Tests.Acceptance.Models.Repositories;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;
using Newtonsoft.Json.Linq;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Repositories
{
    [Collection(nameof(ApiTestCollection))]
    public partial class RepositoriesApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public RepositoriesApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;

        private static IQueryable<Repository> CreateRandomRepositories()
        {
            return CreateRepositoryFiller(DateTimeOffset.UtcNow)
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Repository CreateRandomRepository(DateTimeOffset dateTimeOffset) =>
            CreateRepositoryFiller(dateTimeOffset).Create();

        private static Filler<Repository> CreateRepositoryFiller(DateTimeOffset dateTimeOffset)
        {
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Repository>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(repository => repository.CreatedBy).Use(someUser)
                .OnProperty(repository => repository.UpdatedBy).Use(someUser)
                .OnProperty(repository => repository.Source).IgnoreIt()
                .OnProperty(repository => repository.Contributions).IgnoreIt();

            return filler;
        }

        private async ValueTask<Source> PostRandomSourceAsync()
        {
            Source randomSource = CreateRandomSource(DateTimeOffset.UtcNow);
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);

            return randomSource;
        }

        private static Source CreateRandomSource(DateTimeOffset dateTimeOffset) =>
            CreateSourceFiller(dateTimeOffset).Create();

        private static Filler<Source> CreateSourceFiller(DateTimeOffset dateTimeOffset)
        {
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Source>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(source => source.Url).Use(new RandomUrl().GetValue())
                .OnProperty(source => source.CreatedBy).Use(someUser)
                .OnProperty(source => source.UpdatedBy).Use(someUser)
                .OnProperty(source => source.Repositories).IgnoreIt()
                .OnProperty(source => source.Contributors).IgnoreIt();

            return filler;
        }

    }
}
