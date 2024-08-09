// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        ValueTask LogInformationAsync(string message);
        ValueTask LogTraceAsync(string message);
        ValueTask LogDebugAsync(string message);
        ValueTask LogWarningAsync(string message);
        ValueTask LogErrorAsync(Exception exception);
        ValueTask LogCriticalAsync(Exception exception);
    }
}