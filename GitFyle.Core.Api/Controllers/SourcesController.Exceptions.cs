using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GitFyle.Core.Api.Controllers
{
    public partial class SourcesController
    {
        private delegate ValueTask<ActionResult<Source>> ReturningSourceFunction();
        private delegate ValueTask<ActionResult<IQueryable<Source>>> ReturningSourcesFunction();

        private async ValueTask<ActionResult<Source>> TryCatch(ReturningSourceFunction returningSourceFunction)
        {
            try
            {
                return await returningSourceFunction();
            }
            catch (SourceValidationException sourceValidationException)
              when (sourceValidationException.InnerException is NotFoundSourceException)
            {
                return NotFound(sourceValidationException.InnerException);
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
                when (sourceDependencyValidationException.InnerException is LockedSourceException)
            {
                return Locked(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
            {
                return BadRequest(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyException sourceDependencyException)
            {
                return InternalServerError(sourceDependencyException);
            }
            catch (SourceServiceException sourceServiceException)
            {
                return InternalServerError(sourceServiceException);
            }
        }

        private async ValueTask<ActionResult<IQueryable<Source>>> TryCatch(ReturningSourcesFunction returningSourcesFunction)
        {
            try
            {
                return await returningSourcesFunction();
            }
            catch (SourceDependencyException sourceDependencyException)
            {
                return InternalServerError(sourceDependencyException);
            }
            catch (SourceServiceException sourceServiceException)
            {
                return InternalServerError(sourceServiceException);
            }
        }


    }
}
