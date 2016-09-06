using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using LCore.Extensions;
using LCore.LDoc.Markdown.Manifest;
using LCore.Naming;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Class to describe projects within markdown documents
    /// </summary>
    public class ProjectInfo : INamed
        {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Project description
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Project Url
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// Project LDoc Manifest Url(s)
        /// </summary>
        public string[] LDocTypeManifestUrls { get; set; } = { };

        private LDocTypeManifest[] _Manifests;

        /// <summary>
        /// Loads and caches <see cref="LDocTypeManifest"/> documents for the project
        /// </summary>
        public LDocTypeManifest[] Manifests
            {
            get
                {
                var Client = new WebClient();

                return L.Logic.Cache(ref this._Manifests, () =>
                    {
                        return this.LDocTypeManifestUrls.Convert(Url =>
                            {
                                try
                                    {

                                    string JSON = Client.DownloadString(Url);
                                    return LDocTypeManifest.FromJSON(JSON);
                                    }
                                catch (WebException)
                                    {
                                    return null;
                                    }
                            });
                    });
                }
            }
        }
    }