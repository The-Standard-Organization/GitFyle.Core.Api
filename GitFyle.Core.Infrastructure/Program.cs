// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Infrastructure.Services;

namespace GitFyle.Core.Infrastructure;

internal class Program
{
    static void Main(string[] args)
    {
        var scriptGenerationService = new ScriptGenerationService();
        scriptGenerationService.GenerateBuildScript();
    }
}
