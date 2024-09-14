// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourcesController : RESTFulController
    {
        private readonly ISourceService sourceService;

        public SourcesController(ISourceService sourceService) =>
            this.sourceService = sourceService;

        [HttpPost]
        public async ValueTask<ActionResult<Source>> PostSourceAsync(Source source)
        {
            try
            {
                Source addedSource =
                    await this.sourceService.AddSourceAsync(source);

                return Created(addedSource);
            }
            catch (SourceValidationException sourceValidationException)
            {
                return BadRequest(sourceValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
                when (sourceDependencyValidationException.InnerException is AlreadyExistsSourceException)
            {
                return Conflict(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
            {
                return BadRequest(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyException sourceValidationException)
            {
                return InternalServerError(sourceValidationException);
            }
            catch (SourceServiceException sourceServiceException)
            {
                return InternalServerError(sourceServiceException);
            }
        }
    }
}
