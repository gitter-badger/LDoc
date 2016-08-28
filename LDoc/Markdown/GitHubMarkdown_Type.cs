using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.LUnit;
// ReSharper disable SuggestBaseTypeForParameter

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for a Type.
    /// </summary>
    public class GitHubMarkdown_Type : GitHubMarkdown
        {
        /// <summary>
        /// MetaData for the type
        /// </summary>
        public CodeCoverageMetaData TypeMeta { get; }

        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public GitHubMarkdown_Type(Type Type, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.TypeMeta = Type.GatherCodeCoverageMetaData(Generator.CustomCommentTags);

            this.Generate();
            }


        private void Generate()
            {
            var MarkdownGenerator = this.Generator;

            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Assembly(this.TypeMeta.Member.GetAssembly())), MarkdownGenerator.Language.LinkText_Up));

                this.Header($"{((Type)this.TypeMeta.Member).GetGenericName()}", Size: 3);
                this.Line(MarkdownGenerator.GetBadges_Info(this, new TypeCoverage((Type)this.TypeMeta.Member), this.TypeMeta.Comments).JoinLines(" "));
                this.Line(MarkdownGenerator.GetBadges_Coverage(this, new TypeCoverage((Type)this.TypeMeta.Member), this.TypeMeta.Comments).JoinLines(" "));
                string TypePath = this.TypeMeta.CodeFilePath;

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link(this.GetRelativePath(TypePath), MarkdownGenerator.Language.LinkText_ViewSource));
                    }

                if (!string.IsNullOrEmpty(this.TypeMeta.Comments?.Summary))
                    {
                    this.Header(MarkdownGenerator.Language.Header_Summary, Size: 6);
                    this.Line(this.TypeMeta.Comments?.Summary);
                    }

                MarkdownGenerator.GetTypeMemberMarkdown((Type)this.TypeMeta.Member).Each(Member =>
                   this.Line($" - {this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Member(Member.Key.First())), $"{Member.Key.First()?.Name}")}"));

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }