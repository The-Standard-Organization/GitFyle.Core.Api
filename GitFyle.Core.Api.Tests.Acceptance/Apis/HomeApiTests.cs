// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Tests.Acceptance.Brokers;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class HomeApiTests
    {
        private readonly GitFyleCoreApiBroker gitFyleCoreApiBroker;

        public HomeApiTests(GitFyleCoreApiBroker gitFyleCoreApiBroker) =>
            this.gitFyleCoreApiBroker = gitFyleCoreApiBroker;
    }
}
