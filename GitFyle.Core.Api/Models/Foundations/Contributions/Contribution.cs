// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

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
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset MergedAt { get; set; }
        public ContributionType ContributionType { get; set; }
    }
}
