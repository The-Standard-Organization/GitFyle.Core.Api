// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace GitFyle.Core.Api.Models.Foundations.Configurations
{
    public class Configuration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
