// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace GitFyle.Core.Api.Models.Foundations.Contributor
{
    public class Contributor 
    {

        public Guid Id { get; set; }
        
        public string ExternalId { get; set; }

        public Guid SourceId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Email {  get; set; }

        public string AvatarUrl { get; set; }

    }
}
