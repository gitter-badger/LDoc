using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    ///   Generates markdown for a Member.
    /// </summary>
    public class MarkdownDocument_MethodGroup : GeneratedDocument
        {
        private const BadgeColor InfoColor = BadgeColor.Blue;

        /// <summary>
        ///   Create a new Member Markdown file.
        /// </summary>
        public MarkdownDocument_MethodGroup(MethodInfo[] Members, SolutionMarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            Members.Each(Method => { this.Methods.Add(Method, new CodeCoverageMetaData(Method, Generator.CustomCommentTags)); });
            }

        /// <summary>
        ///   Members and corresponding comments and coverage
        /// </summary>
        public Dictionary<MethodInfo, CodeCoverageMetaData> Methods { get; }
        = new Dictionary<MethodInfo, CodeCoverageMetaData>();

        protected override void GenerateDocument()
            {
            }

        /// <summary>
        ///   Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Info()
            {
            var Out = new List<string>
                {
                this.GetBadge_MemberType(this),
                this.GetBadge_CodeLines(this),
                this.GetBadge_Todos(this),
                this.GetBadge_Bugs(this),
                this.GetBadge_NotImplemented(this),
                this.GetBadge_Documented(this)
                };

            Out.AddRange(this.GetBadge_CustomTags(this));

            return Out;
            }


        /// <summary>
        ///   Get the Documented badge
        /// </summary>
        public string GetBadge_Documented(GitHubMarkdown MD)
            {
            // TODO replace with simpler Percent function
            int PercentageCommented = (int) (this.Methods.Convert(Method => Method.Value.Comments == null
                                                 ? 0
                                                 : 1).Average()*100).Round();

            return MD.Badge(this.Generator.Language.Badge_Documented,
                $"{PercentageCommented}%",
                this.Generator.GetColorByPercentage(PercentageCommented));
            }

        /// <summary>
        ///   Get the Member Type badge (always method)
        /// </summary>
        public string GetBadge_MemberType(GitHubMarkdown MD)
            {
            const MemberType TypeDescription = MemberType.Method;
            return MD.Badge(this.Generator.Language.Badge_Type, TypeDescription.ToString().Pluralize(), InfoColor);
            }

        /// <summary>
        ///   Get badges for custom tracked tags
        /// </summary>
        public string[] GetBadge_CustomTags(GitHubMarkdown MD)
            {
            return this.Generator.CustomCommentTags.Convert(Tag =>
                {
                Func<uint, BadgeColor> CommentColor = this.Generator.CustomCommentColor.SafeGet(Tag);
                uint TagCount = this.Methods.Sum(SubMember => SubMember.Value.CommentTags[Tag].Length);

                var Color = CommentColor?.Invoke(TagCount) ?? InfoColor;

                return MD.Badge(Tag, $"{TagCount}", Color);
                });
            }

        /// <summary>
        ///   Get the Not Implemented badge
        /// </summary>
        public string GetBadge_NotImplemented(GitHubMarkdown MD)
            {
            uint NotImplementedCount = this.Methods.Sum(SubMember => SubMember.Value.NotImplemented.Length);
            return MD.Badge(this.Generator.Language.Badge_NotImplemented, $"{NotImplementedCount}", NotImplementedCount > 0
                ? BadgeColor.Orange
                : BadgeColor.Green);
            }

        /// <summary>
        ///   Get the bugs badge
        /// </summary>
        public string GetBadge_Bugs(GitHubMarkdown MD)
            {
            uint BugCount = this.Methods.Sum(SubMember => SubMember.Value.CommentBUG.Length);
            return MD.Badge(this.Generator.Language.Badge_BUGs, $"{BugCount}", BugCount > 0
                ? BadgeColor.Red
                : BadgeColor.Green);
            }

        /// <summary>
        ///   Get the Todos badge
        /// </summary>
        public string GetBadge_Todos(GitHubMarkdown MD)
            {
            uint TodoCount = this.Methods.Sum(SubMember => SubMember.Value.CommentTODO.Length);
            return MD.Badge(this.Generator.Language.Badge_TODOs, $"{TodoCount}", TodoCount > 0
                ? BadgeColor.Yellow
                : BadgeColor.Green);
            }

        /// <summary>
        ///   Get the Code Lines badge
        /// </summary>
        public string GetBadge_CodeLines(GitHubMarkdown MD)
            {
            return MD.Badge(this.Generator.Language.Badge_LinesOfCode,
                $"{this.Methods.Sum(SubMember => SubMember.Value.CodeLineCount ?? 0u)}", InfoColor);
            }


        /// <summary>
        ///   Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Coverage()
            {
            var Out = new List<string>();

            if (this.Generator.DocumentUnitCoverage)
                Out.Add(this.GetBadge_UnitTests(this));

            if (this.Generator.DocumentAttributeCoverage)
                Out.Add(this.GetBadge_AttributeCoverage(this));

            Out.Add(this.GetBadge_Assertions(this));

            // TODO: Add Test Status: Passing / Failing / Untested

            return Out;
            }


        /// <summary>
        ///   Get the Coverage badge
        /// </summary>
        public string GetBadge_AttributeCoverage(GitHubMarkdown MD)
            {
            uint AttributeTests = this.Methods.Sum(Method => Method.Value.Coverage.AttributeCoverage);

            return MD.Badge(this.Generator.Language.Badge_AttributeTests,
                $"{AttributeTests}", AttributeTests == 0u
                    ? BadgeColor.LightGrey
                    : BadgeColor.BrightGreen);
            }

        /// <summary>
        ///   Get the Unit Tests badge
        /// </summary>
        public string GetBadge_UnitTests(GitHubMarkdown MD)
            {
            // TODO replace with simpler Percent function
            int PercentageCovered = (int) (this.Methods.Convert(Method => Method.Value.Coverage.IsCovered
                                               ? 0
                                               : 1).Average()*100).Round();

            return MD.Badge(this.Generator.Language.Badge_UnitTested,
                $"{PercentageCovered}", this.Generator.GetColorByPercentage(PercentageCovered));
            }

        /// <summary>
        ///   Get the Assertions badge
        /// </summary>
        public string GetBadge_Assertions(GitHubMarkdown MD)
            {
            uint TotalAssertions = this.Methods.Sum(Method => Method.Value.Coverage.AssertionsMade);
            return MD.Link(MD.GetRelativePath(this.Methods.First().Value.CodeFilePath),
                MD.Badge(this.Generator.Language.Badge_Assertions,
                    $"{TotalAssertions}", TotalAssertions > 0u
                        ? BadgeColor.BrightGreen
                        : BadgeColor.LightGrey), EscapeText: false);
            }
        }
    }