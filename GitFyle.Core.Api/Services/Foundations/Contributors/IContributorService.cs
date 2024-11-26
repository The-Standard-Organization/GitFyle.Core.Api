// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;

namespace GitFyle.Core.Api.Services.Foundations.Contributors
{
    public interface IContributorService
    {
        ValueTask<Contributor> AddContributorAsync(Contributor contributor);
        ValueTask<IQueryable<Contributor>> RetrieveAllContributorsAsync();
        ValueTask<Contributor> RetrieveContributorByIdAsync(Guid contributorId);
        ValueTask<Contributor> ModifyContributorAsync(Contributor contributor);
        ValueTask<Contributor> RemoveContributorByIdAsync(Guid contributorId);
    }
}