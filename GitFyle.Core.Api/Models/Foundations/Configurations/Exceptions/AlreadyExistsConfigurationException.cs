using System;
using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class AlreadyExistsConfigurationException : Xeption
    {
        public AlreadyExistsConfigurationException(
            string message, 
            Exception innerException, 
            IDictionary data) :
            base(message, 
                innerException,
                data)
        { }
    }
}
