// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Repositories;

namespace GitFyle.Core.Api.Models.Foundations.Contributions
{
    public class Contribution
    {
        public Guid Id { get; set; }
        public Guid RepositoryId { get; set; }
        public Guid ContributorId { get; set; }
        public Guid ContributionTypeId { get; set; }
        public string ExternalId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset ExternalCreatedAt { get; set; }
        public DateTimeOffset ExternalUpdatedAt { get; set; }
        public DateTimeOffset ExternalMergedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedWhen { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedWhen { get; set; }
        public ContributionType ContributionType { get; set; }
        public Contributor Contributor { get; set; }
        public Repository Repository { get; set; }
    }
}
