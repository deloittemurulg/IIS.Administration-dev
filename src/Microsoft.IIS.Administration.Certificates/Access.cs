// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.Certificates
{
    using System;

    [Flags]
    public enum CertificateAccess
    {
        Read = 1,
        Delete = 2,
        Create = 4,
        Export = 8
    }
}
