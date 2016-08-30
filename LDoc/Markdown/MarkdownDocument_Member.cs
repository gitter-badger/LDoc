using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
{
    /// <summary>
    /// Generates documentation for a Member
    /// </summary>
    public class MarkdownDocument_Member : GitHubMarkdown
    {
        private const BadgeColor InfoColor = BadgeColor.Blue;

        /// <summary>
        /// The markdown generator that created this document
        /// </summary>
        public SolutionMarkdownGenerator SolutionMarkdownGenerator { get; }

        /// <summary>
        /// Members and corresponding comments and coverage
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Metadata for the current <see cref="Member"/>
        /// </summary>
        public CodeCoverageMetaData Meta { get; }

        /// <summary>
        /// Creates a new markdown member document
        /// </summary>
        public MarkdownDocument_Member(MemberInfo Member, SolutionMarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
        {
            this.Member = Member;

            this.Meta = Member.GatherCodeCoverageMetaData(Generator.CustomCommentTags);

            this.SolutionMarkdownGenerator = this.Generator;
        }

        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        protected List<string> GetBadges_Info([CanBeNull] MethodCoverage Coverage)
        {
            var Out = new List<string>
            {
                this.GetBadge_MemberType(this),
                this.GetBadge_CodeLines(this),
                this.GetBadge_Todos(this),
                this.GetBadge_Bugs(this),
                this.GetBadge_NotImplemented(this),
                this.GetBadge_Documented(this),
                this.GetBadge_SourceCode(this)
            };

            Out.AddRange(this.GetBadge_CustomTags(this));

            return Out;
        }

        /// <summary>
        /// Generate the document
        /// </summary>
        public void Generate()
        {
            this.SolutionMarkdownGenerator.WriteHeader(this);
            this.Line(this.Link(this.GetRelativePath(this.SolutionMarkdownGenerator.MarkdownPath_Type(this.Member.DeclaringType)), this.SolutionMarkdownGenerator.Language.LinkText_Up));

            this.Header($"{this.Member.DeclaringType?.Name}", Size: 3);

            string TypePath = this.Member.DeclaringType.FindClassFile();

            if (!string.IsNullOrEmpty(TypePath))
            {
                this.Line(this.Link(this.GetRelativePath(TypePath), this.SolutionMarkdownGenerator.Language.LinkText_ViewSource));
            }


            this.Header(this.Member.Name);

            if (this.Member is MethodInfo)
            {
                var Method = (MethodInfo) this.Member;

                string Static = Method.IsStatic
                    ? "Static "
                    : "Instance";

                string StaticLower = Method.IsStatic
                    ? " static"
                    : "";

                string Parameters = Method.GetParameters()
                    .Convert(Param => $"{this.SolutionMarkdownGenerator.LinkToType(this, Param.ParameterType)} {Param.Name}")
                    .Combine(", ");

                this.Header($"{Static}Method", Size: 4);


                this.Header(
                    $"public{StaticLower} {this.SolutionMarkdownGenerator.LinkToType(this, Method.ReturnType)} {this.Member.Name}({Parameters});",
                    Size: 6);

                this.Line("");
                this.Line(this.GetBadges_Info(this.Meta.Coverage).JoinLines(" "));
                this.Line("");
                this.Line(this.GetBadges_Coverage().JoinLines(" "));

                // TODO: Add test coverage link

                if (this.Meta.Comments?.Summary != null)
                {
                    this.Header(this.SolutionMarkdownGenerator.Language.Header_Summary, Size: 5);
                    this.Line(this.Generator?.FormatComment(this.Meta.Comments?.Summary));
                }

                if (Method.GetParameters().Length > 0)
                {
                    this.Header(this.SolutionMarkdownGenerator.Language.Header_MethodParameters, Size: 6);

                    var Table = new List<string[]>
                    {
                        new[]
                        {
                            this.SolutionMarkdownGenerator.Language.TableHeaderText_MethodParameter, this.SolutionMarkdownGenerator.Language.TableHeaderText_Optional, this.SolutionMarkdownGenerator.Language.TableHeaderText_Type, this.SolutionMarkdownGenerator.Language.TableHeaderText_Description
                        }
                    };

                    Method.GetParameters().Each((ParamIndex, Param) =>
                    {
                        Table.Add(new[]
                        {
                            Param.Name,
                            Param.IsOptional
                                ? "Yes"
                                : "No",
                            this.SolutionMarkdownGenerator.LinkToType(this, Param.ParameterType), this.Meta.Comments?.Parameters.GetAt(ParamIndex)?.Obj2
                        });
                    });

                    this.Table(Table);
                }

                this.Header(this.SolutionMarkdownGenerator.Language.Header_MethodReturns, Size: 4);

                this.Header(this.SolutionMarkdownGenerator.LinkToType(this, Method.ReturnType), Size: 6);

                this.Line(this.Meta.Comments?.Returns);

                if (this.Meta.Comments?.Examples.Length > 0)
                {
                    this.Header(this.SolutionMarkdownGenerator.Language.Header_MethodExamples, Size: 4);
                    this.Meta.Comments?.Examples.Each(Example => this.Code(new[] {Example}));
                }

                if (this.Meta.Comments?.Permissions.Length > 0)
                {
                    this.Header(this.SolutionMarkdownGenerator.Language.Header_MethodPermissions, Size: 4);
                    this.Meta.Comments?.Permissions.Each(Permission => this.Line($"{Permission.Obj1} {Permission.Obj2}"));
                }

                if (this.Meta.Comments?.Exceptions.Length > 0)
                {
                    this.Header(this.SolutionMarkdownGenerator.Language.Header_MethodExceptions, Size: 4);
                    this.Meta.Comments?.Exceptions.Each(Exception => this.Line($"{Exception.Obj1} {Exception.Obj2}"));
                }
            }

            this.SolutionMarkdownGenerator.WriteFooter(this);
        }

        /// <summary>
        /// Get the Documented badge
        /// </summary>
        public string GetBadge_Documented(GitHubMarkdown MD)
        {
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_Documented,
                this.Meta.Comments != null
                    ? "Yes"
                    : "No",
                this.Meta.Comments != null
                    ? BadgeColor.BrightGreen
                    : BadgeColor.Red);
        }

        /// <summary>
        /// Get the Member Type badge
        /// </summary>
        public string GetBadge_MemberType(GitHubMarkdown MD)
        {
            var TypeDescription = this.Member.GetMemberDetails();
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_Type, TypeDescription.ToString(), InfoColor);
        }

        /// <summary>
        /// Get the Member Source Code badge
        /// </summary>
        public string GetBadge_SourceCode(GitHubMarkdown MD)
        {
            return string.IsNullOrEmpty(this.Meta.CodeFilePath)
                ? MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_SourceCode, this.SolutionMarkdownGenerator.Language.Badge_SourceCodeUnavailable,
                    BadgeColor.Red)
                : MD.Link($"{MD.GetRelativePath(this.Meta.CodeFilePath)}#L{this.Meta.CodeLineNumber}",
                    MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_SourceCode, this.SolutionMarkdownGenerator.Language.Badge_SourceCodeAvailable,
                        BadgeColor.BrightGreen), EscapeText: false);
        }

        /// <summary>
        /// Get the Not Implemented badge
        /// </summary>
        public string GetBadge_NotImplemented(GitHubMarkdown MD)
        {
            uint NotImplementedCount = (uint) this.Meta.NotImplemented.Length;
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_NotImplemented, $"{NotImplementedCount}", NotImplementedCount > 0
                ? BadgeColor.Orange
                : BadgeColor.Green);
        }

        /// <summary>
        /// Get the Code Lines badge
        /// </summary>
        public string GetBadge_CodeLines(GitHubMarkdown MD)
        {
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_LinesOfCode, $"{this.Meta.CodeLineCount}");
        }

        /// <summary>
        /// Get the Todos badge
        /// </summary>
        public string GetBadge_Todos(GitHubMarkdown MD)
        {
            uint TodoCount = (uint) this.Meta.CommentTODO.Length;
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_TODOs, $"{TodoCount}", TodoCount > 0
                ? BadgeColor.Yellow
                : BadgeColor.Green);
        }

        /// <summary>
        /// Get the bugs badge
        /// </summary>
        public string GetBadge_Bugs(GitHubMarkdown MD)
        {
            uint BugCount = (uint) this.Meta.CommentBUG.Length;
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_BUGs, $"{BugCount}", BugCount > 0
                ? BadgeColor.Red
                : BadgeColor.Green);
        }

        /// <summary>
        /// Get badges for custom tracked tags
        /// </summary>
        public string[] GetBadge_CustomTags(GitHubMarkdown MD)
        {
            return this.SolutionMarkdownGenerator.CustomCommentTags.Convert(Tag =>
            {
                Func<uint, BadgeColor> CommentColor = this.SolutionMarkdownGenerator.CustomCommentColor.SafeGet(Tag);
                uint TagCount = (uint) this.Meta.CommentTags[Tag].Length;

                var Color = CommentColor?.Invoke(TagCount) ?? InfoColor;

                return MD.Badge(Tag, $"{TagCount}", Color);
            });
        }

        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        protected virtual List<string> GetBadges_Coverage()
        {
            var Out = new List<string>();

            if (this.Member != null)
            {
                if (this.SolutionMarkdownGenerator.DocumentUnitCoverage)
                    Out.Add(this.GetBadge_UnitTests(this));

                if (this.SolutionMarkdownGenerator.DocumentAttributeCoverage)
                    Out.Add(this.GetBadge_AttributeCoverage(this));

                Out.Add(this.GetBadge_Assertions(this));

                // TODO: Add Test Status: Passing / Failing / Untested
            }
            return Out;
        }

        /// <summary>
        /// Get the Coverage badge
        /// </summary>
        public string GetBadge_AttributeCoverage(GitHubMarkdown MD)
        {
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_AttributeTests,
                $"{this.Meta.Coverage.AttributeCoverage}",
                this.Meta.Coverage.AttributeCoverage > 0u
                    ? BadgeColor.BrightGreen
                    : BadgeColor.LightGrey);
        }

        /// <summary>
        /// Get the Unit Tests badge
        /// </summary>
        public string GetBadge_UnitTests(GitHubMarkdown MD)
        {
            return MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_UnitTested, this.Meta.Coverage.MemberTraitFound
                ? "Yes"
                : "No", this.Meta.Coverage.MemberTraitFound
                ? BadgeColor.BrightGreen
                : BadgeColor.LightGrey);
        }

        /// <summary>
        /// Get the Assertions badge
        /// </summary>
        public string GetBadge_Assertions(GitHubMarkdown MD)
        {
            return MD.Link(MD.GetRelativePath(this.Meta.CodeFilePath),
                MD.Badge(this.SolutionMarkdownGenerator.Language.Badge_Assertions, $"{this.Meta.Coverage.AssertionsMade}", this.Meta.Coverage.AssertionsMade > 0u
                    ? BadgeColor.BrightGreen
                    : BadgeColor.LightGrey), EscapeText: false);
        }
    }
}