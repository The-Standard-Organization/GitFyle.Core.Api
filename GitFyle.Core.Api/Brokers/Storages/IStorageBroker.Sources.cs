// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Source> InsertSourceAsync(Source source);
        IQueryable<Source> SelectAllSources();
        ValueTask<Source> SelectSourceByIdAsync(Guid sourceId);
        ValueTask<Source> UpdateSourceAsync(Source source);
        ValueTask<Source> DeleteSourceAsync(Source source);
    }
}
