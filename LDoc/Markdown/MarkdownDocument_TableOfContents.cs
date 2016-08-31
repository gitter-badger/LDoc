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

            this.Generator.GetAllMarkdown().Each(Document => { this.Line($" - {this.Link(this.GetRelativePath(Document.FilePath), Document.Title)}"); });

            this.Generator.WriteFooter(this);
            }
        }
    }