using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        public CodeCoverageMetaData TypeMeta { get; set; }

        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public GitHubMarkdown_Type(Type Type, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.TypeMeta = Type.GatherCodeCoverageMetaData(/*Generator.CustomCommentTags*/);

            this.Generate();
            }


        private void Generate()
            {
            var MarkdownGenerator = this.Generator;

            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Assembly(TypeMeta.Type.GetAssembly())), MarkdownGenerator.Language.LinkText_Up));

                this.Header($"{TypeMeta.Type.Name}", Size: 3);
                this.Line(MarkdownGenerator.GetBadges_Info(this, TypeMeta.Coverage, TypeMeta.Comments).JoinLines(" "));
                this.Line(MarkdownGenerator.GetBadges_Coverage(this, TypeMeta.Coverage, TypeMeta.Comments).JoinLines(" "));
                string TypePath = TypeMeta.CodeFilePath;

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link(this.GetRelativePath(TypePath), MarkdownGenerator.Language.LinkText_ViewSource));
                    }

                if (!string.IsNullOrEmpty(TypeMeta.Comments?.Summary))
                    {
                    this.Header(MarkdownGenerator.Language.Header_Summary, Size: 6);
                    this.Line(TypeMeta.Comments.Summary);
                    }

                MarkdownGenerator.GetTypeMemberMarkdown(TypeMeta.Type).Each(Member =>
                    this.Line($" - {this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Member(Member.Key.First())), $"{Member.Key.First()?.Name}")}"));

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }