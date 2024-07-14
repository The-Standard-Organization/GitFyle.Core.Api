// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private void ValidateSourceOnAdd(Source source)
        {
            ValidateSourceIsNotNull(source);
        }

        private static void ValidateSourceIsNotNull(Source source)
        {
            if (source is null)
            {
                throw new NullSourceException(message: "Source is null");
            }
        }
    }
}