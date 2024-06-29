// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace GitFyle.Core.Api.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        void LogTrace(string message);
        void LogDebug(string message);
    }
}
