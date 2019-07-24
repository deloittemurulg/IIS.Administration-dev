﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.Core
{
    using System.Collections.Generic;

    public interface INonsensitiveAuditingFields
    {
        void Add(string field);
        IEnumerable<string> Value { get; }
    }
}
