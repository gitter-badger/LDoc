using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

// ReSharper disable SuggestBaseTypeForParameter

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for the a project's table of contents.
    /// </summary>
    public class MarkdownDocument_CoverageSummary : GeneratedDocument
        {
        /// <summary>
        /// Create a new root Markdown file.
        /// </summary>
        public MarkdownDocument_CoverageSummary(SolutionMarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            }

        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.WriteHeader(this);

            this.Line(this.Header(this.Generator.Language.CoverageSummary));

            // TODO Generate summary markdown

            this.Line(this.Header(this.Generator.Language.Header_Assemblies));
            this.Generator.Markdown_Assembly.Each(AssemblyMD =>
            {
                var Coverage = new AssemblyCoverage(AssemblyMD.Key);
                ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                this.Line(this.Header(AssemblyMD.Value.Title, Size: 2));
                //MD.Line(this.GetBadges_Info(MD, Coverage, Comments).JoinLines(" "));
                // ReSharper disable once ExpressionIsAlwaysNull
                this.Line(AssemblyMD.Value.GetBadges_Coverage(this, Coverage, Comments).JoinLines(" "));
            });

            this.Generator.WriteFooter(this);
            }
        }
    }