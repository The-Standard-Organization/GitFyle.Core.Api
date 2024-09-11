// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal interface ISourceService
    {
        ValueTask<Source> AddSourceAsync(Source source);
        ValueTask<Source> RetrieveSourceByIdAsync(Guid sourceId);
        ValueTask<IQueryable<Source>> RetrieveAllSourcesAsync();
        ValueTask<Source> ModifySourceAsync(Source source);
        ValueTask<Source> RemoveSourceByIdAsync(Guid sourceId);
    }
}