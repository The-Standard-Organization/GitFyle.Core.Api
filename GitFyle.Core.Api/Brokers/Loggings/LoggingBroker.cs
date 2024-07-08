﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;

namespace GitFyle.Core.Api.Brokers.Loggings;

public class LoggingBroker : ILoggingBroker
{
    private readonly ILogger<LoggingBroker> logger;

    public LoggingBroker(ILogger<LoggingBroker> logger) =>
        this.logger = logger;

    public void LogInformation(string message) => 
        this.logger.LogInformation(message);
        
    public void LogTrace(string message) =>
        this.logger.LogTrace(message);

    public void LogDebug(string message) =>
        this.logger.LogDebug(message);

    public void LogWarning(string message) =>
        this.logger.LogWarning(message);
        
    public void LogError(Exception exception) =>
       this.logger.LogError(exception, message: exception.Message);

    public void LogCritical(Exception exception) =>
       this.logger.LogCritical(exception, message: exception.Message);
}
