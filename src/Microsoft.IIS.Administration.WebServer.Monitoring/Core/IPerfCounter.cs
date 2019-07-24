﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.Monitoring
{
    public interface IPerfCounter
    {
        string Name { get; }
        string InstanceName { get; }
        string CategoryName { get; }
        string Path { get; }
        long Value { get; set; }
    }
}
