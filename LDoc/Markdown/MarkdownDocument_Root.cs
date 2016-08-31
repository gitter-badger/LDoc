using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            this.Generator.WriteHeader(this);
            this.Line(this.Header(this.Generator.Language.MainReadme, Size: 2));

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
                // TODO pull assembly comments from override
                ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                this.Line(this.Header(Document.Value.Title, Size: 2));
                //MD.Line(this.GetBadges_Info(MD, Coverage, Comments).JoinLines(" "));
                this.Line(this.Generator.GetBadges_Coverage(this, Coverage, Comments).JoinLines(" "));

                this.Line($" - {this.Link(this.GetRelativePath(Document.Value.FilePath), Document.Value.Title)}");
            });

            if (!this.Generator.Home_RelatedProjects.IsEmpty())
                this.Line(this.Header(this.Generator.Language.Header_RelatedProjects, Size: 3));

            this.UnorderedList(
                this.Generator.Home_RelatedProjects.Convert(
                    Project => $"{this.Link(Project.Url, Project.Name)} {Project.Description}").Array());

            this.Generator.WriteFooter(this);
            }
        }
    }