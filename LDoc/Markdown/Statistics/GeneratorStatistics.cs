using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Stores statistics for markdown generation
    /// </summary>
    public class GeneratorStatistics
        {
        /// <summary>
        /// Total documents generated
        /// </summary>
        public uint MarkdownDocuments { get; set; }

        /// <summary>
        /// Project documents generated
        /// </summary>
        public uint ProjectMarkdownDocuments { get; set; }
        /// <summary>
        /// Assembly documents generated
        /// </summary>
        public uint AssemblyMarkdownDocuments { get; set; }
        /// <summary>
        /// Type documents generated
        /// </summary>
        public uint TypeMarkdownDocuments { get; set; }
        /// <summary>
        /// Member documents generated
        /// </summary>
        public uint MemberMarkdownDocuments { get; set; }

        /// <summary>
        /// Lines generated
        /// </summary>
        public uint Lines { get; set; }
        /// <summary>
        /// Headers generated
        /// </summary>
        public uint Headers { get; set; }
        /// <summary>
        /// Tables generated
        /// </summary>
        public uint Tables { get; set; }
        /// <summary>
        /// Links generated
        /// </summary>
        public uint Links { get; set; }
        /// <summary>
        /// Local links generated
        /// </summary>
        public uint LocalLinks { get; set; }
        /// <summary>
        /// System links generated
        /// </summary>
        public uint SystemLinks { get; set; }
        /// <summary>
        /// External links generated
        /// </summary>
        public uint ExternalLinks { get; set; }
        /// <summary>
        /// LDoc links included via linked LDoc manifests
        /// </summary>
        public uint LDocLinks { get; set; }

        /// <summary>
        /// Badges generated
        /// </summary>
        public uint Badges { get; set; }

        /// <summary>
        /// Defaults to the time the <see cref="SolutionMarkdownGenerator"/> and <see cref="GeneratorStatistics"/> was created.
        /// </summary>
        public DateTime StartGenerationTime { get; } = DateTime.Now;

        /// <summary>
        /// Ensures that <see cref="EndGenerationTime"/> is accurate whenever it is retrieved.
        /// </summary>
        public DateTime EndGenerationTime => DateTime.Now;

        /// <summary>
        /// Retrieves the running duration of generation
        /// </summary>
        public TimeSpan Duration => this.EndGenerationTime - this.StartGenerationTime;


        /// <summary>
        /// Retrieves a list of tables representing statistics gathered.
        /// </summary>
        public List<string[,]> ToTables()
            {
            var Out = new List<string[,]>
                {
                new[,]
                    {
                        {"Generation Time", "Total"},
                        {nameof(this.Duration).Humanize(), $"{this.Duration.ToTimeString()}"}
                   //     {"Per Document", $"{TimeSpan.FromMilliseconds(this.Duration.TotalMilliseconds /this.MarkdownDocuments).ToTimeString()}"},
                   //     {"Per Line", $"{TimeSpan.FromMilliseconds(this.Duration.TotalMilliseconds /this.Lines).ToTimeString()}"}
                    },
                new[,]
                    {
                        {"Documents", "Total"},
                        {nameof(this.MarkdownDocuments).Humanize(), $"{this.MarkdownDocuments}"},
                        {nameof(this.ProjectMarkdownDocuments).Humanize(), $"{this.ProjectMarkdownDocuments}"},
                        {nameof(this.AssemblyMarkdownDocuments).Humanize(), $"{this.AssemblyMarkdownDocuments}"},
                        {nameof(this.TypeMarkdownDocuments).Humanize(), $"{this.TypeMarkdownDocuments}"},
                        {nameof(this.MemberMarkdownDocuments).Humanize(), $"{this.MemberMarkdownDocuments}"}
                    },
                new[,]
                    {
                        {"Markdown", "Total"},
                        {nameof(this.Lines).Humanize(), $"{this.Lines}"},
                        {nameof(this.Headers).Humanize(), $"{this.Headers}"},
                        {nameof(this.Tables).Humanize(), $"{this.Tables}"},
                        {nameof(this.Badges).Humanize(), $"{this.Badges}"}
                    },
                new[,]
                    {
                        {"Links", "Total"},
                        {nameof(this.Links).Humanize(), $"{this.Links}"},
                        {nameof(this.LocalLinks).Humanize(), $"{this.LocalLinks}"},
                        {nameof(this.SystemLinks).Humanize(), $"{this.SystemLinks}"},
                        {"LDoc Links", $"{this.LDocLinks}"},
                        {nameof(this.ExternalLinks).Humanize(), $"{this.ExternalLinks}"}
                    }
                };
            return Out;
            }
        }
    }