// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using System.Linq;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    public interface IContributionService
    {
        ValueTask<Contribution> AddContributionAsync(Contribution contribution);
        ValueTask<IQueryable<Contribution>> RetrieveAllContributionsAsync();
    }
}
