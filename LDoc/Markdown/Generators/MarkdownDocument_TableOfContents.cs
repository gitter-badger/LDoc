using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;

// ReSharper disable SuggestBaseTypeForParameter

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for the a project's table of contents.
    /// </summary>
    public class MarkdownDocument_TableOfContents : GeneratedDocument
        {
        public string AnchorLink_Statistics;
        public string AnchorLink_Errors;

        /// <summary>
        /// Create a new root Markdown file.
        /// </summary>
        public MarkdownDocument_TableOfContents(SolutionMarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            }

        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.WriteHeader(this);

            this.Line(this.Header(this.Generator.Language.TableOfContents, Size: 2));

            this.Generator.GetAllMarkdown().Each(Document =>
                this.Line($" - {this.Link(this.GetRelativePath(Document.FilePath), Document.Title)}"));

            this.BlankLine();

            this.Line(this.HeaderAnchor(this.Generator.Language.TableHeader_Statistics, out this.AnchorLink_Statistics, Size: 3));
            this.BlankLine();
            List<string[,]> Stats = this.Generator.Stats.ToTables();

            foreach (string[,] StatTable in Stats)
                this.Table(StatTable);

            List<string> Errors = this.Generator.GetErrors();

            if (!Errors.IsEmpty())
                {
                this.Line(this.HeaderAnchor($"{this.Generator.Language.TableHeader_Errors} ({Errors.Count})", out this.AnchorLink_Errors, Size: 3));

                Errors.Each(Error => this.Line($"- {Error}"));
                }

            this.Generator.WriteFooter(this);
            }

        }
    }