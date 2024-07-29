﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    public interface ISourceService
    {
        ValueTask<Source> AddSourceAsync(Source source);
    }
}
