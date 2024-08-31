using GitFyle.Core.Api.Models.Foundations.Contributions;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    public interface IContributionService
    {
        ValueTask<Contribution> AddContributionAsync(Contribution contribution);
    }
}
