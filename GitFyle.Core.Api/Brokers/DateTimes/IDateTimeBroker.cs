// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------
using System;

namespace GitFyle.Core.Api.Brokers.DateTimes;

public interface IDateTimeBroker
{
    DateTimeOffset GetCurrentDateTimeOffset();
}
