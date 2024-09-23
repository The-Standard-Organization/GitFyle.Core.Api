using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class NotFoundConfigurationException : Xeption
    {
        public NotFoundConfigurationException(string message)
            : base(message)
        { }
    }
}
