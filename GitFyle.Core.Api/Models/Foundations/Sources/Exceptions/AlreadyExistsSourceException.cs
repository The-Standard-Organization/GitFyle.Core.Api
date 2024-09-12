﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class AlreadyExistsSourceException : Xeption
    {
        public AlreadyExistsSourceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}