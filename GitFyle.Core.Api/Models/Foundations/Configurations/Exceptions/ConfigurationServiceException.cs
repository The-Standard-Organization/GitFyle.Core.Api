using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class ConfigurationServiceException : Xeption
    {
        public ConfigurationServiceException(string message, Xeption innerException) :
            base(message, innerException)
        { }
    }
}
