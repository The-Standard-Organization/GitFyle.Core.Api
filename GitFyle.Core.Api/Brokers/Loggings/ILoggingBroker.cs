using System;


namespace GitFyle.Core.Api.Brokers.Loggings
{
    public interface ILoggingBroker 
    {
        void LogInInformation(string message);
    }
}
