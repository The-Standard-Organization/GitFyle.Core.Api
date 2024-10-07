// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string HomeUrl = "api/home";

        public async ValueTask<string> GetHomeApiMessageAsync() =>
            await this.apiFactoryClient.GetContentStringAsync(HomeUrl);
    }
}
