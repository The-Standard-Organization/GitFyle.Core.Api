// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Models.Foundations.Contributors
{
    public class Contributor 
    { 
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public Guid SourceId { get; set; }
        public string Name { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Source Source { get; set; }
        public string Username { get; set; }
        public string Email {  get; set; }
        public string AvatarUrl { get; set; }       
    }
}