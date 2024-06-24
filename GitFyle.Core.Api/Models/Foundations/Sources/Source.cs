// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace GitFyle.Core.Api.Models.Foundations.Sources
{
    public class Source
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
