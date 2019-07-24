// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    using System;
    using Web.Administration;

    sealed class RewriteMapCollection : ConfigurationElementCollectionBase<RewriteMap> {
        
        public new RewriteMap this[string rewriteMapName] {
            get {
                for (int i = 0; i < Count; i++) {
                    RewriteMap element = base[i];
                    if (String.Equals(rewriteMapName, element.Name, StringComparison.OrdinalIgnoreCase)) {
                        return element;
                    }
                }
                return null;
            }
        }

        protected override RewriteMap CreateNewElement(string elementTagName) {
            return new RewriteMap();
        }

    }
}

