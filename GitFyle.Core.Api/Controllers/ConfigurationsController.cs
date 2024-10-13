using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationsController : RESTFulController
    {
        private readonly IConfigurationService configurationService;

        public ConfigurationsController(IConfigurationService configurationService) =>
            this.configurationService = configurationService;

        [HttpPost]
        public async ValueTask<ActionResult<Configuration>> PostConfigurationAsync(Configuration configuration)
        {
            try
            {
                Configuration addedConfiguration =
                    await this.configurationService.AddConfigurationAsync(configuration);

                return Created(addedConfiguration);
            }
            catch (ConfigurationValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
                when (configurationDependencyValidationException.InnerException is AlreadyExistsConfigurationException)
            {
                return Conflict(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyException configurationDependencyException)
            {
                return InternalServerError(configurationDependencyException);
            }
            catch (ConfigurationServiceException configurationServiceException)
            {
                return InternalServerError(configurationServiceException);
            }
        }

        [HttpGet("{configurationId}")]
        public async ValueTask<ActionResult<Configuration>> GetConfigurationByIdAsync(Guid configurationId)
        {
            try
            {
                Configuration configuration =
                    await this.configurationService.RetrieveConfigurationByIdAsync(configurationId);

                return Ok(configuration);
            }
            catch (ConfigurationValidationException configurationValidationException)
                when (configurationValidationException.InnerException is NotFoundConfigurationException)
            {
                return NotFound(configurationValidationException.InnerException);
            }
            catch (ConfigurationValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
        }
    }
}
