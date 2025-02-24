// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string SourceRelativeUrl = "api/sources";

        public async ValueTask<Source> PostSourceAsync(Source source) =>
            await this.apiFactoryClient.PostContentAsync(SourceRelativeUrl, source);

        public async ValueTask<Source> DeleteSourceByIdAsync(Guid sourceId) =>
            await this.apiFactoryClient.DeleteContentAsync<Source>($"{SourceRelativeUrl}/{sourceId}");
    }
}
