using System;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
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
