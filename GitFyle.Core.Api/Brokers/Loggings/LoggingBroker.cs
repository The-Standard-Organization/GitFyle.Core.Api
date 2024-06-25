using Microsoft.Extensions.Logging;

namespace GitFyle.Core.Api.Brokers.Loggings
{
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger logger;

        public LoggingBroker(ILogger logger) =>
            this.logger = logger;   

        public void LogInInformation(string message) =>
            this.logger.LogInformation(message);

    }
}
