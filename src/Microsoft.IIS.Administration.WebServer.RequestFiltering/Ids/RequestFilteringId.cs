// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration.WebServer.RequestFiltering
{
    using System;

    public class RequestFilteringId
    {
        private const string PURPOSE = "WebServer.RequestFiltering";
        private const char DELIMITER = '\n';

        private const uint PATH_INDEX = 0;
        private const uint SITE_ID_INDEX = 1;
        private const uint IS_LOCAL_INDEX = 2;

        public bool IsLocal { get; private set; }
        public string Path { get; private set; }
        public long? SiteId { get; private set; }
        public string Uuid { get; private set; }

        public RequestFilteringId(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) {
                throw new ArgumentNullException("uuid");
            }

            var info = Core.Utils.Uuid.Decode(uuid, PURPOSE).Split(DELIMITER);

            string path = info[PATH_INDEX];
            string siteId = info[SITE_ID_INDEX];
            string isLocal = info[IS_LOCAL_INDEX];

            // Check if request filtering specifies a path
            // If request filtering belongs to some path it must also provide site id
            if (!string.IsNullOrEmpty(path)) {
                if (string.IsNullOrEmpty(siteId)) {
                    throw new ArgumentNullException("siteId");
                }
                this.Path = path;
                this.SiteId = long.Parse(siteId);
            }
            // Check if request filtering belongs to site
            else if (!string.IsNullOrEmpty(siteId)) {
                this.SiteId = long.Parse(siteId);
            }

            this.IsLocal = int.Parse(isLocal) == 1;
            this.Uuid = uuid;
        }

        public RequestFilteringId(long? siteId, string path, bool isLocal)
        {

            if (!string.IsNullOrEmpty(path) && siteId == null) {
                throw new ArgumentNullException("siteId");
            }

            this.Path = path;
            this.SiteId = siteId;
            this.IsLocal = isLocal;

            string encodableSiteId = this.SiteId == null ? "" : this.SiteId.Value.ToString();
            string encodableIsLocal = (this.IsLocal ? 1 : 0).ToString();

            this.Uuid = Core.Utils.Uuid.Encode($"{this.Path}{DELIMITER}{encodableSiteId}{DELIMITER}{encodableIsLocal}", PURPOSE);
        }
    }
}
