// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Validations;
using Xeptions;

namespace GitFyle.Core.Api.Brokers.DateTimes
{
    public partial class ValidationBroker : IValidationBroker
    {
        public DateTimeOffset GetCurrentDateTimeOffset() =>
            DateTimeOffset.UtcNow;

        public async ValueTask Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption =>
            await ValidationEngine.Validate<T>(message, validations);
    }
}
