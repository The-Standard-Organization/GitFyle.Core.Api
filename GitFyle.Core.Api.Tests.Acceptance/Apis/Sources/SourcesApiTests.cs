// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Sources
{
    public partial class SourcesApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public SourcesApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;

        private static Source CreateRandomSource(DateTimeOffset dateTimeOffset) =>
            CreateSourceFiller(dateTimeOffset).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<Source> PostRandomSourceAsync()
        {
            Source randomSource = CreateRandomSource(DateTimeOffset.UtcNow);
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);

            return randomSource;
        }

        private static Source UpdateSourceRandom(Source source)
        {
            var now = DateTimeOffset.UtcNow;
            source.UpdatedDate = now;

            return source;
        }

        private static IQueryable<Source> CreateRandomSources()
        {
            return CreateSourceFiller(DateTimeOffset.UtcNow)
                .Create(GetRandomNumber())
                .AsQueryable();
        }

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
