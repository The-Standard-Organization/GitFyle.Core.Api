using GitFyle.Core.Api.Models.Foundations.Sources;
using System;
using System.ComponentModel.DataAnnotations;

namespace GitFyle.Core.Api.Models.Foundations.Repositories;

public class Repository
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Owner { get; set; }

    [Required]
    [MaxLength(255)]
    public string ExternalId { get; set; }
    public Guid SourceId { get; set; }
    public bool IsOrganization { get; set; }
    public bool IsPrivate { get; set; }
    public string Token { get; set; }
    public DateTimeOffset TokenExpireAt { get; set; }
    public string Description { get; set; }

    //Navigations
    public Source Source { get; set; }
}
