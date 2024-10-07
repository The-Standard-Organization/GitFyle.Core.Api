// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis
{
    public partial class HomeApiTests
    {
        [Fact]
        public async Task ShouldGetHomeMessageAsync()
        {
            // given
            string expectedHomeMessage =
                "Hello, Mario. The princess is in another castle.";

            // when
            string actualHomeMessage =
                await this.gitFyleCoreApiBroker
                    .GetHomeApiMessageAsync();

            // then
            actualHomeMessage.Should().Be(expectedHomeMessage);
        }
    }
}
