// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------
using System.Threading.Tasks;
using Xeptions;

namespace GitFyle.Core.Api.Brokers.DateTimes
{
    public partial interface IValidationBroker
    {
        ValueTask Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption;
    }
}
