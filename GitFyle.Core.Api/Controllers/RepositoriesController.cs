// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Services.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : RESTFulController
    {
        private readonly IRepositoryService repositoryService;

        public RepositoriesController(IRepositoryService repositoryService) =>
            this.repositoryService = repositoryService;

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Repository>>> GetRepositoriesAsync()
        {
                IQueryable<Repository> repositories =
                    await this.repositoryService.RetrieveAllRepositoriesAsync();

                return Ok(repositories);
        }
    }
}
