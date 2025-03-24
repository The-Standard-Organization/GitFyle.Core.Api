// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Services.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContributionTypesController : RESTFulController
    {
        private readonly IContributionTypeService contributionTypeService;

        public ContributionTypesController(IContributionTypeService contributionTypeService) =>
            this.contributionTypeService = contributionTypeService;

        [HttpPost]
        public async ValueTask<ActionResult<ContributionType>> PostContributionTypeAsync(ContributionType contributionType)
        {
                ContributionType addedContributionType =
                    await this.contributionTypeService.AddContributionTypeAsync(contributionType);

                return Created(addedContributionType);
        }

    }
}
