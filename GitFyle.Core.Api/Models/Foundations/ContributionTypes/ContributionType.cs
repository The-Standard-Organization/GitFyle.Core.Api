// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes;

public class ContributionType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
    public IEnumerable<Contribution> Contributions { get; set; }
}
