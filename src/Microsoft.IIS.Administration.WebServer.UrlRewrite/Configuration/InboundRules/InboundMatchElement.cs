// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    sealed class InboundMatchElement : MatchElement {

        public string Pattern {
            get {
                return ((string)(base["url"]));
            }
            set {
                base["url"] = value;
            }
        }
    }
}

