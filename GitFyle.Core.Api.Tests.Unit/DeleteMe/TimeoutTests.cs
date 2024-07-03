// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Tests.Unit.DeleteMe
{
    public partial class TimeoutTests
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask MustCompleteWithinTimeout(
            ReturningNothingFunction returningNothingFunction,
            int timeoutMilliseconds)
        {
            using var cts = new CancellationTokenSource();
            var timeoutTask = Task.Delay(timeoutMilliseconds, cts.Token);
            var testTask = Task.Run(async () => await returningNothingFunction(), cts.Token);
            var completedTask = await Task.WhenAny(testTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                cts.Cancel();
                throw new TimeoutException($"The test exceeded the allowed timeout period of {timeoutMilliseconds}ms.");
            }

            await testTask;
        }
    }
}
