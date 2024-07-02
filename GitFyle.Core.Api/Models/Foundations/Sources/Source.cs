// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Repositories;

namespace GitFyle.Core.Api.Models.Foundations.Sources
{
    public class Source
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IEnumerable<Contributor> Contributors { get; set; }
        public IEnumerable<Repository> Repositories { get; set; }
    }
}