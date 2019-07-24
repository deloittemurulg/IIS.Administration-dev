﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.UrlRewrite
{
    using Microsoft.IIS.Administration.Core;
    using Microsoft.IIS.Administration.Core.Utils;
    using Microsoft.Web.Administration;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;

    static class RewriteMapsHelper
    {
        public static readonly Fields SectionRefFields = new Fields("id", "scope");
        public static readonly Fields RewriteMapRefFields = new Fields("name", "id");

        public static string GetSectionLocation(string id)
        {
            return $"/{Defines.REWRITE_MAPS_SECTION_PATH}/{id}";
        }

        public static string GetMapLocation(string id)
        {
            return $"/{Defines.REWRITE_MAPS_PATH}/{id}";
        }

        public static RewriteMapsSection GetSection(Site site, string path, string configPath = null)
        {
            return (RewriteMapsSection)ManagementUnit.GetConfigSection(site?.Id,
                                                                           path,
                                                                           Globals.RewriteMapSectionName,
                                                                           typeof(RewriteMapsSection),
                                                                           configPath);
        }

        public static bool IsSectionLocal(Site site, string path)
        {
            return ManagementUnit.IsSectionLocal(site?.Id,
                                                 path,
                                                 Globals.RewriteMapSectionName);
        }

        public static object SectionToJsonModelRef(Site site, string path, Fields fields = null)
        {
            if (fields == null || !fields.HasFields) {
                return SectionToJsonModel(site, path, SectionRefFields, false);
            }
            else {
                return SectionToJsonModel(site, path, fields, false);
            }
        }

        public static object SectionToJsonModel(Site site, string path, Fields fields = null, bool full = true)
        {
            if (fields == null) {
                fields = Fields.All;
            }

            RewriteId id = new RewriteId(site?.Id, path);
            var section = GetSection(site, path);

            dynamic obj = new ExpandoObject();

            //
            // id
            if (fields.Exists("id")) {
                obj.id = id.Uuid;
            }

            //
            // scope
            if (fields.Exists("scope")) {
                obj.scope = site == null ? string.Empty : site.Name + path;
            }

            //
            // metadata
            if (fields.Exists("metadata")) {
                obj.metadata = ConfigurationUtility.MetadataToJson(section.IsLocallyStored, section.IsLocked, section.OverrideMode, section.OverrideModeEffective);
            }

            //
            // url_rewrite
            if (fields.Exists("url_rewrite")) {
                obj.url_rewrite = RewriteHelper.ToJsonModelRef(site, path);
            }

            return Core.Environment.Hal.Apply(Defines.RewriteMapsSectionResource.Guid, obj, full);
        }

        public static object MapToJsonModelRef(RewriteMap map, Site site, string path, Fields fields = null)
        {
            if (fields == null || !fields.HasFields) {
                return MapToJsonModel(map, site, path, RewriteMapRefFields, false);
            }
            else {
                return MapToJsonModel(map, site, path, fields, false);
            }
        }

        public static object MapToJsonModel(RewriteMap map, Site site, string path, Fields fields = null, bool full = true)
        {
            if (map == null) {
                return null;
            }

            if (fields == null) {
                fields = Fields.All;
            }

            var rewriteMapId = new RewriteMapId(site?.Id, path, map.Name);
            var section = GetSection(site, path);

            dynamic obj = new ExpandoObject();

            //
            // name
            if (fields.Exists("name")) {
                obj.name = map.Name;
            }

            //
            // id
            if (fields.Exists("id")) {
                obj.id = rewriteMapId.Uuid;
            }

            //
            // default_value
            if (fields.Exists("default_value")) {
                obj.default_value = map.DefaultValue;
            }

            //
            // ignore_case
            if (fields.Exists("ignore_case")) {
                obj.ignore_case = map.IgnoreCase;
            }

            //
            // mappings
            if (fields.Exists("mappings")) {
                obj.mappings = map.KeyValuePairCollection.Select(kvp => new {
                    name = kvp.Key,
                    value = kvp.Value
                });
            }

            //
            // url_rewrite
            if (fields.Exists("url_rewrite")) {
                obj.url_rewrite = RewriteHelper.ToJsonModelRef(site, path, fields.Filter("url_rewrite"));
            }

            return Core.Environment.Hal.Apply(Defines.RewriteMapsResource.Guid, obj, full);
        }

        public static void UpdateSection(dynamic model, Site site, string path, string configPath = null)
        {
            if (model == null) {
                throw new ApiArgumentException("model");
            }

            RewriteMapsSection section = GetSection(site, path, configPath);

            try {
                if (model.metadata != null) {
                    DynamicHelper.If<OverrideMode>((object)model.metadata.override_mode, v => {
                        section.OverrideMode = v;
                    });
                }
            }
            catch (FileLoadException e) {
                throw new LockedException(section.SectionPath, e);
            }
            catch (DirectoryNotFoundException e) {
                throw new ConfigScopeNotFoundException(e);
            }
        }

        public static void UpdateMap(dynamic model, RewriteMap map, RewriteMapsSection section)
        {
            SetMap(model, map, section);
        }

        public static RewriteMap CreateMap(dynamic model, RewriteMapsSection section)
        {
            if (model == null) {
                throw new ApiArgumentException("model");
            }

            if (string.IsNullOrEmpty(DynamicHelper.Value(model.name))) {
                throw new ApiArgumentException("name");
            }

            RewriteMap map = section.RewriteMaps.CreateElement();

            SetMap(model, map, section);

            return map;
        }

        public static void AddMap(RewriteMap map, RewriteMapsSection section)
        {
            if (map == null) {
                throw new ArgumentNullException(nameof(map));
            }

            if (map.Name == null) {
                throw new ArgumentNullException("map.Name");
            }

            if (section.RewriteMaps.Any(m => m.Name.Equals(map.Name))) {
                throw new AlreadyExistsException("map");
            }

            try {
                section.RewriteMaps.Add(map);
            }
            catch (FileLoadException e) {
                throw new LockedException(section.SectionPath, e);
            }
            catch (DirectoryNotFoundException e) {
                throw new ConfigScopeNotFoundException(e);
            }
        }

        public static void DeleteMap(RewriteMap map, RewriteMapsSection section)
        {
            if (map == null) {
                return;
            }

            map = section.RewriteMaps.FirstOrDefault(r => r.Name.Equals(map.Name));

            if (map != null) {
                try {
                    section.RewriteMaps.Remove(map);
                }
                catch (FileLoadException e) {
                    throw new LockedException(section.SectionPath, e);
                }
                catch (DirectoryNotFoundException e) {
                    throw new ConfigScopeNotFoundException(e);
                }
            }
        }

        private static void SetMap(dynamic model, RewriteMap map, RewriteMapsSection section)
        {
            if (model == null) {
                throw new ApiArgumentException("model");
            }

            try {
                //
                // Name, check for already existing name
                string name = DynamicHelper.Value(model.name);
                if (!string.IsNullOrEmpty(name)) {
                    if (!name.Equals(map.Name, StringComparison.OrdinalIgnoreCase) &&
                            section.RewriteMaps.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) {
                        throw new AlreadyExistsException("name");
                    }

                    map.Name = name;
                }

                DynamicHelper.If((object)model.default_value, v => map.DefaultValue = v);
                DynamicHelper.If<bool>((object)model.ignore_case, v => map.IgnoreCase = v);

                //
                // mappings
                if (model.mappings != null) {

                    IEnumerable<dynamic> mappings = model.mappings as IEnumerable<dynamic>;

                    if (mappings == null) {
                        throw new ApiArgumentException("mappings", ApiArgumentException.EXPECTED_ARRAY);
                    }

                    map.KeyValuePairCollection.Clear();

                    foreach (dynamic item in mappings) {
                        if (!(item is JObject)) {
                            throw new ApiArgumentException("mappings.item");
                        }

                        string itemName = DynamicHelper.Value(item.name);
                        string value = DynamicHelper.Value(item.value);

                        if (string.IsNullOrEmpty(itemName)) {
                            throw new ApiArgumentException("mappings.item.name", "Required");
                        }

                        if (string.IsNullOrEmpty(value)) {
                            throw new ApiArgumentException("mappings.item.value", "Required");
                        }

                        KeyValueElement kvp = map.KeyValuePairCollection.CreateElement();
                        kvp.Key = itemName;
                        kvp.Value = value;

                        map.KeyValuePairCollection.Add(kvp);
                    }
                }
            }
            catch (FileLoadException e) {
                throw new LockedException(section.SectionPath, e);
            }
            catch (DirectoryNotFoundException e) {
                throw new ConfigScopeNotFoundException(e);
            }
        }
    }
}
