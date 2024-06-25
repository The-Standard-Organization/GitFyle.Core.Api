using System;


namespace GitFyle.Core.Api.Brokers.Loggings
{
    public interface ILoggingBroker 
    {
        void LogInformation(string message);
    }
}
