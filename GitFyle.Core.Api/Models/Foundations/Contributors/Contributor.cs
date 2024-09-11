// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Models.Foundations.Contributors
{
    public class Contributor : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public Guid SourceId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public DateTimeOffset ExternalCreatedAt { get; set; }
        public DateTimeOffset ExternalUpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Source Source { get; set; }
        public IEnumerable<Contribution> Contributions { get; set; }
    }
}