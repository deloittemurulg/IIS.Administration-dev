// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    using System;
    using Web.Administration;

    class RuleCollectionBase : ConfigurationElementCollectionBase<RuleElement> {

        public RuleElement Add(string name) {
            RuleElement element = this.CreateElement();
            element.Name = name;
            return base.Add(element);
        }

        public new RuleElement this[string name] {
            get {
                for (int i = 0; (i < this.Count); i = (i + 1)) {
                    RuleElement element = base[i];
                    if ((string.Equals(element.Name, name, StringComparison.OrdinalIgnoreCase) == true)) {
                        return element;
                    }
                }
                return null;
            }
        }

        public new int IndexOf(RuleElement rule)
        {
            int index = -1;

            for (int i = 0; i < Count; i++) {
                if (this[i].Name.Equals(rule.Name, StringComparison.OrdinalIgnoreCase)) {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public virtual void Move(RuleElement rule, int index)
        {
            if (index < 0 || index >= this.Count) {
                throw new IndexOutOfRangeException();
            }

            int currentIndex = IndexOf(rule);

            if (currentIndex == -1) {
                throw new ArgumentException(nameof(rule));
            }

            //
            // Make sure rule comes from this collection instance
            RuleElement r = this[currentIndex];
            this.RemoveAt(currentIndex);
            this.AddCopyAt(index, r);
        }

        protected virtual void CopyInfo(RuleElement source, RuleElement destination) {
            source.Action.CopyTo(destination.Action);

            source.Conditions.CopyTo(destination.Conditions);

            source.Match.CopyTo(destination.Match);

            ConfigurationHelper.CopyAttributes(source, destination);

            ConfigurationHelper.CopyMetadata(source, destination);
        }

        private RuleElement AddCopyAt(int index, RuleElement rule)
        {
            RuleElement element = CreateElement();

            CopyInfo(rule, element);

            return AddAt(index, element);
        }
    }

    static class ConfigurationHelper {

        public static void CopyAttributes(ConfigurationElement source, ConfigurationElement destination) {
            foreach (ConfigurationAttribute attribute in source.Attributes) {
                if (!attribute.IsInheritedFromDefaultValue) {
                    destination[attribute.Name] = attribute.Value;
                }
            }
        }

        public static void CopyMetadata(ConfigurationElement source, ConfigurationElement destination) {
            object o = source.GetMetadata("lockItem");
            if (o != null) {
                destination.SetMetadata("lockItem", o);
            }

            o = source.GetMetadata("lockAttributes");
            if (o != null) {
                destination.SetMetadata("lockAttributes", o);
            }

            o = source.GetMetadata("lockElements");
            if (o != null) {
                destination.SetMetadata("lockElements", o);
            }

            o = source.GetMetadata("lockAllAttributesExcept");
            if (o != null) {
                destination.SetMetadata("lockAllAttributesExcept", o);
            }

            o = source.GetMetadata("lockAllElementsExcept");
            if (o != null) {
                destination.SetMetadata("lockAllElementsExcept", o);
            }
        }

    }
}

