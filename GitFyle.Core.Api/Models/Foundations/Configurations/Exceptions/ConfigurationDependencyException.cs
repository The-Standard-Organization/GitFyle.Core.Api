using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class ConfigurationDependencyException : Xeption
    {
        public ConfigurationDependencyException(string message, Xeption innerException) :
            base(message, innerException)
        { }
    }
}
