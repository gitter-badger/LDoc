using System;
using System.Collections;
using System.Collections.Generic;
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
        public string[] LDocManifestUrls { get; set; } = { };
        }
    }