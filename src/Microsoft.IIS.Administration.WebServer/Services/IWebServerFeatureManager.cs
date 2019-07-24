﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer
{
    using System.Threading.Tasks;

    public interface IWebServerFeatureManager
    {
        Task Enable(params string[] features);

        Task Disable(params string[] features);
    }
}
