// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    sealed class InboundRule : RuleElement {

        public const string RedirectTypeAttribute = "redirectType";
        public const string ResponseCacheDirectiveAttribute = "responseCacheDirective";

        private InboundActionElement _action;
        private InboundMatchElement _match;
        private SetCollection _sets;

        public new InboundActionElement Action {
            get {
                if ((this._action == null)) {
                    this._action = ((InboundActionElement)(base.GetChildElement("action", typeof(InboundActionElement))));
                }
                return this._action;
            }
        }

        public new InboundMatchElement Match {
            get {
                if ((this._match == null)) {
                    this._match = ((InboundMatchElement)(base.GetChildElement("match", typeof(InboundMatchElement))));
                }
                return this._match;
            }
        }

        public SetCollection ServerVariableAssignments {
            get {
                if ((this._sets == null)) {
                    this._sets = ((SetCollection)(base.GetCollection("serverVariables", typeof(SetCollection))));
                }
                return this._sets;
            }
        }

        public ResponseCacheDirective ResponseCacheDirective {
            get {
                return ((ResponseCacheDirective)(base["responseCacheDirective"]));
            }
            set {
                base["responseCacheDirective"] = ((int)(value));
            }
        }
    }
}

