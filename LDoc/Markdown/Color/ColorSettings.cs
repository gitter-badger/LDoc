using System;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Class to store color settings for markdown generation
    /// </summary>
    public class ColorSettings
        {
        /// <summary>
        /// The color used for information badges
        /// </summary>
        public string BadgeInfoColor { get; set; } = $"{BadgeColor.Blue}";
        }
    }