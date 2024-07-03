// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Tests.Unit.DeleteMe
{
    public partial class TimeoutTests
    {
        [Fact]
        public async Task ShouldCompleteWithinTimeoutAsync()
        {
            await MustCompleteWithinTimeout(async () =>
            {
                await Task.Delay(900);
            },
            timeoutMilliseconds: 1000);
        }

        [Fact]
        public async Task ShouldFailToCompleteWithinTimeoutAsync()
        {
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await MustCompleteWithinTimeout(async () =>
                {
                    await Task.Delay(1100);
                },
                timeoutMilliseconds: 1000);
            });
        }
    }
}
