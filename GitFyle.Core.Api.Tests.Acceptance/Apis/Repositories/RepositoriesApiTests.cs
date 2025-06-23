// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using GitFyle.Core.Api.Tests.Acceptance.Models.Repositories;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Repositories
{
    [Collection(nameof(ApiTestCollection))]
    public partial class RepositoriesApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public RepositoriesApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;

        private async Task<List<Repository>> PostRandomRepositoriesAsync(Guid sourceId)
        {
            List<Repository> repositories = CreateRandomRepositories(sourceId).ToList();

            foreach (var repository in repositories)
            {
                await this.gitFyleCoreApiBroker.PostRepositoryAsync(repository);
            }

            return repositories;
        }

        private static List<Repository> CreateRandomRepositories(Guid sourceId)
        {
            return CreateRepositoryFiller(sourceId)
                .Create(GetRandomNumber())
                .ToList();
        }

        private async ValueTask<Repository> ModifyRandomRepository(Guid sourceId)
        {
            Repository randomRepository = await PostRandomRepository(sourceId);
            randomRepository.UpdatedDate = DateTime.UtcNow;
            randomRepository.UpdatedBy = Guid.NewGuid().ToString();

            return randomRepository;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
           new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private async ValueTask<Repository> PostRandomRepository(Guid sourceId)
        {
            Repository randomRepository = CreateRandomRepository(sourceId);
            await this.gitFyleCoreApiBroker.PostRepositoryAsync(randomRepository);

            return randomRepository;
        }

        private static Repository CreateRandomRepository(Guid sourceId) =>
            CreateRepositoryFiller(sourceId).Create();

        private static Filler<Repository> CreateRepositoryFiller(Guid sourceId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Repository>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(repository => repository.SourceId).Use(sourceId)
                .OnProperty(repository => repository.CreatedBy).Use(someUser)
                .OnProperty(repository => repository.UpdatedBy).Use(someUser)
                .OnProperty(repository => repository.Source).IgnoreIt()
                .OnProperty(repository => repository.Contributions).IgnoreIt();

            return filler;
        }

        private async ValueTask<Source> PostRandomSourceAsync()
        {
            Source randomSource = CreateRandomSource();
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);

            return randomSource;
        }

        private static Source CreateRandomSource() =>
            CreateSourceFiller().Create();

        private static Filler<Source> CreateSourceFiller()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Source>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(source => source.Url).Use(new RandomUrl().GetValue())
                .OnProperty(source => source.CreatedBy).Use(someUser)
                .OnProperty(source => source.UpdatedBy).Use(someUser)
                .OnProperty(source => source.Repositories).IgnoreIt()
                .OnProperty(source => source.Contributors).IgnoreIt();

            return filler;
        }
    }
}