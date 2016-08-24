using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for a Type.
    /// </summary>
    public class GitHubMarkdown_Type : GitHubMarkdown
        {
        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Type comments
        /// </summary>
        public ICodeComment Comments { get; }

        /// <summary>
        /// Type coverage
        /// </summary>
        public TypeCoverage Coverage { get; }

        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public GitHubMarkdown_Type(Type Type, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Type = Type;
            this.Comments = Type.GetComments();
            this.Coverage = new TypeCoverage(Type);

            this.Generate();
            }

        private void Generate()
            {
            var MarkdownGenerator = this.Generator;

            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                var Coverage = new TypeCoverage(this.Type);
                var Comments = this.Type.GetComments();

                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Assembly(this.Type.GetAssembly())), MarkdownGenerator.Language.LinkText_Up));

                this.Header($"{this.Type.Name}", Size: 3);
                this.Line(MarkdownGenerator.GetBadges_Info(this, Coverage, Comments).JoinLines(" "));
                this.Line(MarkdownGenerator.GetBadges_Coverage(this, Coverage, Comments).JoinLines(" "));
                string TypePath = this.Type.FindClassFile();

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link(this.GetRelativePath(TypePath), MarkdownGenerator.Language.LinkText_ViewSource));
                    }

                if (!string.IsNullOrEmpty(Comments?.Summary))
                    {
                    this.Header(MarkdownGenerator.Language.Header_Summary, Size: 6);
                    this.Line(Comments.Summary);
                    }

                MarkdownGenerator.GetTypeMemberMarkdown(this.Type).Each(Member =>
                    this.Line($" - {this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Member(Member.Key.First())), $"{Member.Key.First()?.Name}")}"));

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }