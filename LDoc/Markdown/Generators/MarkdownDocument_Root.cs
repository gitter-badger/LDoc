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
    /// Generates markdown for the root file of a project.
    /// </summary>
    public class MarkdownDocument_Root : GeneratedDocument
        {
        /// <summary>
        /// Create a new root Markdown file.
        /// </summary>
        public MarkdownDocument_Root(SolutionMarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            }

        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.ProjectMarkdownDocuments++;

            this.Generator.WriteHeader(this);
            this.Line(this.Header(this.Generator.Language.MainReadme, Size: 2));

            if (!this.Generator.CustomBadgeUrls.IsEmpty())
                this.Line(this.Generator.CustomBadgeUrls.Convert(Url => this.Image(Url)).JoinLines(" "));

            this.Generator.Home_Intro(this);

            if (!this.GetType().GetMembers().Has(
                Member => Member.IsDeclaredMember() &&
                          (Member.Name == nameof(this.Generator.HowToInstall))))
                {
                this.Line(this.Header(this.Generator.Language.Header_InstallationInstructions, Size: 3));

                this.Generator.HowToInstall(this);
                }

            this.Generator.Markdown_Assembly.Each(Document =>
            {
                var Coverage = new AssemblyCoverage(Document.Key);

                ICodeComment Comments = this.Generator.AssemblyComments.SafeGet(Document.Key);

                this.Line(this.Header(Document.Value.Title, Size: 2));
                //MD.Line(this.GetBadges_Info(MD, Coverage, Comments).JoinLines(" "));
                // ReSharper disable once ExpressionIsAlwaysNull
                this.Line(Document.Value.GetBadges_Coverage(this, Coverage, Comments).JoinLines(" "));

                this.Line($" - {this.Link(this.GetRelativePath(Document.Value.FilePath), Document.Value.Title)}");
            });

            if (!this.Generator.Home_RelatedProjects.IsEmpty())
                this.Line(this.Header(this.Generator.Language.Header_RelatedProjects, Size: 3));

            this.UnorderedList(
                this.Generator.Home_RelatedProjects.Convert(
                    Project => $"{this.Link(Project.Url, Project.Name)} {Project.Description}").Array());


            var TodoDocument = this.Generator.Markdown_Other.SafeGet("TODO Summary");
            if (TodoDocument != null)
                this.Line(this.Header(this.Link(this.GetRelativePath(TodoDocument.FilePath), TodoDocument.Title), Size: 3));

            var BugDocument = this.Generator.Markdown_Other.SafeGet("BUG Summary");
            if (BugDocument != null)
                {

                }

            this.Generator.WriteFooter(this);
            }
        }
    }