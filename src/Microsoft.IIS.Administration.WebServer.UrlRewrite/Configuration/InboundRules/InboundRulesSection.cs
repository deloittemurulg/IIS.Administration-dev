// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    using Web.Administration;

    sealed class InboundRulesSection : ConfigurationSection {

        public const string UseOriginalUrlEncodingAttribute = "useOriginalURLEncoding";

        private InboundRuleCollection _rules;

        public bool UseOriginalURLEncoding {
            get {
                return ((bool)(base[UseOriginalUrlEncodingAttribute]));
            }
            set {
                base[UseOriginalUrlEncodingAttribute] = value;
            }
        }

        public InboundRuleCollection InboundRules {
            get {
                if ((this._rules == null)) {
                    this._rules = ((InboundRuleCollection)(base.GetCollection(typeof(InboundRuleCollection))));
                }
                return this._rules;
            }
        }
    }
}

