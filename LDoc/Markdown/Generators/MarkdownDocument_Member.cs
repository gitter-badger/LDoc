using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates documentation for a Member
    /// </summary>
    public class MarkdownDocument_Member : GeneratedDocument
        {
        #region Properties

        /// <summary>
        /// Members and corresponding comments and coverage
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Metadata for the current <see cref="Member"/>
        /// </summary>
        public CodeCoverageMetaData Meta { get; }

        #endregion

        /// <summary>
        /// Creates a new markdown member document
        /// </summary>
        public MarkdownDocument_Member(MemberInfo Member, SolutionMarkdownGenerator Generator, string Title)
            : base(Generator, Title)
            {
            this.Member = Member;

            this.Meta = Member.GatherCodeCoverageMetaData(Generator.CustomCommentTags);
            }

        /// <inheritdoc />
        protected override string FileName =>
            $"{this.Member.DeclaringType?.Name.CleanFileName()}_{this.Member.Name.CleanFileName()}{this.Generator.GetMethodIndex(this.Member)}.md";

        /// <inheritdoc />
        protected override string FilePath => this.Generator.MarkdownPath_MemberRoot(this.Member);

        /// <summary>
        /// Generate the document
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.MemberMarkdownDocuments++;

            this.Line(this.Header($"namespace {this.Meta.Type.Namespace}", Size: 6));
            this.Line(this.Header($"{this.Meta.Type.GetMemberDetails()?.ToCodeString()} " +
                                  $"{this.Link(this.GetRelativePath(this.Generator.Markdown_Type[this.Meta.Type].FullPath), this.Meta.Type.GetGenericName())}", Size: 6));

            this.Line(this.Link(this.GetRelativePath(
                this.Generator.Markdown_Type[this.Member.DeclaringType].FullPath), this.Generator.Language.LinkText_Up));

            this.Line(this.Header($"{this.Member.DeclaringType?.Name}", Size: 3));

            string TypePath = this.Member.DeclaringType.FindClassFile();

            if (!string.IsNullOrEmpty(TypePath))
                {
                this.Line(this.Link(this.GetRelativePath(TypePath), this.Generator.Language.LinkText_ViewSource));
                }

            this.Line(this.Header(this.Member.Name));

            if (this.Meta.Comments?.Summary != null)
                {
                this.Line(this.Header(this.Generator.Language.Header_Summary, Size: 5));
                this.Line(this.Generator.FormatComment(this, this.Meta.Comments?.Summary));
                }

            // TODO display attributes

            #region MethodInfo

            if (this.Member is MethodInfo)
                {
                var Method = (MethodInfo)this.Member;
                var Details = Method.GetMemberDetails();

                string Signature = this.GetSignature(this, AsHtml: true);
                this.Line(this.Header($"{Details}", Size: 4));
                this.Line(this.Header(Signature, Size: 5));

                this.Line("");
                this.Line(this.GetBadges_Info().JoinLines(" "));
                this.Line("");
                this.Line(this.GetBadges_Coverage().JoinLines(" "));


                if (Method.GetParameters().Length > 0)
                    {
                    this.Line(this.Header(this.Generator.Language.Header_MethodParameters, Size: 6));

                    var Table = new List<string[]>
                        {
                        new[]
                            {
                            this.Generator.Language.TableHeaderText_MethodParameter, this.Generator.Language.TableHeaderText_Optional, this.Generator.Language.TableHeaderText_Type, this.Generator.Language.TableHeaderText_Description
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
                            this.Generator.LinkToType(this, Param.ParameterType), this.Meta.Comments?.Parameters.GetAt(ParamIndex)?.Obj2
                            });
                        });

                    this.Table(Table);
                    }

                this.Line(this.Header(this.Generator.Language.Header_MethodReturns, Size: 4));

                this.Line(this.Header(this.Generator.LinkToType(this, Method.ReturnType), Size: 6));

                this.Line(this.Meta.Comments?.Returns);

                if (this.Meta.Comments?.Examples.Length > 0)
                    {
                    this.Line(this.Header(this.Generator.Language.Header_MethodExamples, Size: 4));
                    this.Meta.Comments?.Examples.Each(Example => this.Code(new[] { Example }));
                    }

                if (this.Meta.Comments?.Permissions.Length > 0)
                    {
                    this.Line(this.Header(this.Generator.Language.Header_MethodPermissions, Size: 4));
                    this.Meta.Comments?.Permissions.Each(Permission => this.Line($"{Permission.Obj1} {Permission.Obj2}"));
                    }

                if (this.Meta.Comments?.Exceptions.Length > 0)
                    {
                    this.Line(this.Header(this.Generator.Language.Header_MethodExceptions, Size: 4));
                    this.Meta.Comments?.Exceptions.Each(Exception => this.Line($"{Exception.Obj1} {Exception.Obj2}"));
                    }

                // TODO coverage section
                }

            #endregion

            #region PropertyInfo

            if (this.Member is PropertyInfo)
                {
                // TODO document for PropertyInfo
                }

            #endregion

            #region FieldInfo

            if (this.Member is FieldInfo)
                {
                // TODO document for FieldInfo
                }

            #endregion
            }

        /// <summary>
        /// Returns the markdown string representing a Member's signature.
        /// Excluding links, appears like:
        /// 
        /// public string GetSignature(GitHubMarkdown MD, bool AsHtml)
        /// 
        /// </summary>
        public virtual string GetSignature(GeneratedDocument MD, bool AsHtml = false)
            {
            bool Remote = MD != this;
            var Details = this.Member.GetMemberDetails();
            if (Details != null)
                {
                if (this.Member is MethodInfo)
                    {
                    var Method = (MethodInfo)this.Member;

                    bool ShowInheritance = Details.Inheritance != MemberInheritance.None;

                    string Parameters = Method.GetParameters()
                        .Convert(Param => $"{this.Generator.LinkToType(this, Param.ParameterType, AsHtml)} {Param.Name}")
                        .Combine(", ");

                    string Name = Remote
                        ? MD.Link(MD.GetRelativePath(this.FilePath), this.Member.Name, AsHtml: AsHtml)
                        : this.Member.Name;

                    return $"{Details.Scope.ToString().ToLower()} {(ShowInheritance ? $"{Details.Inheritance.ToString().ToLower()} " : "")}" +
                           $"{(Method.IsStatic ? "static " : "")}" +
                           $"{this.Generator.LinkToType(this, Method.ReturnType, AsHtml)} " +
                           $"{Name}({Parameters});";
                    }
                }

            return "";
            }

        #region Info Badges

        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        protected List<string> GetBadges_Info()
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
        /// Get the Documented badge
        /// </summary>
        public string GetBadge_Documented(GeneratedDocument MD, bool AsHtml = false)
            {
            return MD.Badge(this.Generator.Language.Badge_Documented,
                this.Meta.Comments != null
                    ? "Yes"
                    : "No",
                this.Meta.Comments != null
                    ? BadgeColor.BrightGreen
                    : BadgeColor.Red, AsHtml);
            }

        /// <summary>
        /// Get the Member Type badge
        /// </summary>
        public string GetBadge_MemberType(GeneratedDocument MD, bool AsHtml = false)
            {
            var TypeDescription = this.Member.GetMemberDetails();
            return MD.Badge(this.Generator.Language.Badge_Type, TypeDescription?.ToString(),
                this.Generator.Colors.BadgeInfoColor, AsHtml);
            }

        /// <summary>
        /// Get the Member Source Code badge
        /// </summary>
        public string GetBadge_SourceCode(GeneratedDocument MD, bool AsHtml = false)
            {
            if (string.IsNullOrEmpty(this.Meta.CodeFilePath))
                {
                this.Generator.AddError(
                    $"Could not find code file path for {this.Meta.Member.FullyQualifiedName()}");
                return "";
                }

            return string.IsNullOrEmpty(this.Meta.CodeFilePath)
                ? MD.Badge(this.Generator.Language.Badge_SourceCode, this.Generator.Language.Badge_SourceCodeUnavailable,
                    BadgeColor.Red)
                : MD.Link($"{MD.GetRelativePath(this.Meta.CodeFilePath)}#L{this.Meta.CodeLineNumber}",
                    MD.Badge(this.Generator.Language.Badge_SourceCode, this.Generator.Language.Badge_SourceCodeAvailable,
                        BadgeColor.BrightGreen), EscapeText: false, AsHtml: AsHtml);
            }

        /// <summary>
        /// Get the Not Implemented badge
        /// </summary>
        public string GetBadge_NotImplemented(GeneratedDocument MD, bool AsHtml = false)
            {
            uint NotImplementedCount = (uint)this.Meta.NotImplemented.Length;

            if (NotImplementedCount == 0)
                return "";

            return MD.Badge(this.Generator.Language.Badge_NotImplemented, $"{NotImplementedCount}", NotImplementedCount > 0
                ? BadgeColor.Orange
                : BadgeColor.Green, AsHtml);
            }

        /// <summary>
        /// Get the Code Lines badge
        /// </summary>
        public string GetBadge_CodeLines(GeneratedDocument MD, bool AsHtml = false)
            {
            if (this.Meta.CodeLineNumber == null || this.Meta.CodeLineNumber <= 0u)
                {
                this.Generator.AddError(
                    $"Could not find code line number for {this.Meta.Member.FullyQualifiedName()}");
                return "";
                }
            if (this.Meta.CodeLineCount == null || this.Meta.CodeLineCount == 0u)
                {
                this.Generator.AddError(
                    $"Could not find code line count for {this.Meta.Member.FullyQualifiedName()}");
                return "";
                }

            return MD.Link($"{MD.GetRelativePath(this.Meta.CodeFilePath)}#L{this.Meta.CodeLineNumber}",
                MD.Badge(this.Generator.Language.Badge_LinesOfCode,
                    $"{this.Meta.CodeLineCount ?? 0u}",
                    (this.Meta.CodeLineCount ?? 0u) == 0u
                        ? BadgeColor.Red
                        : BadgeColor.Blue, AsHtml), EscapeText: false, AsHtml: AsHtml);
            }

        /// <summary>
        /// Get the Todos badge
        /// </summary>
        public string GetBadge_Todos(GeneratedDocument MD, bool AsHtml = false)
            {
            uint TodoCount = (uint)this.Meta.CommentTODO.Length;

            if (TodoCount == 0)
                return "";

            return MD.Badge(this.Generator.Language.Badge_TODOs, $"{TodoCount}", TodoCount > 0
                ? BadgeColor.Yellow
                : BadgeColor.Green, AsHtml);
            }

        /// <summary>
        /// Get the bugs badge
        /// </summary>
        public string GetBadge_Bugs(GeneratedDocument MD, bool AsHtml = false)
            {
            uint BugCount = (uint)this.Meta.CommentBUG.Length;
            if (BugCount == 0)
                return "";

            return MD.Badge(this.Generator.Language.Badge_BUGs, $"{BugCount}", BugCount > 0
                ? BadgeColor.Red
                : BadgeColor.Green, AsHtml);
            }

        /// <summary>
        /// Get badges for custom tracked tags
        /// </summary>
        public string[] GetBadge_CustomTags(GeneratedDocument MD, bool AsHtml = false)
            {
            return this.Generator.CustomCommentTags.Convert(Tag =>
                {
                    Func<uint, BadgeColor> CommentColor = this.Generator.CustomCommentColor.SafeGet(Tag);
                    uint TagCount = (uint)this.Meta.CommentTags[Tag].Count;

                    string Color = $"{CommentColor?.Invoke(TagCount)}";
                    if (string.IsNullOrEmpty(Color))
                        Color = this.Generator.Colors.BadgeInfoColor;

                    return MD.Badge(Tag, $"{TagCount}", Color, AsHtml);
                });
            }

        #endregion

        #region Coverage Badges

        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        protected virtual List<string> GetBadges_Coverage()
            {
            var Out = new List<string>();

            if (this.Member != null)
                {
                if (this.Generator.DocumentUnitCoverage)
                    Out.Add(this.GetBadge_Covered(this));

                if (this.Generator.DocumentUnitCoverage)
                    Out.Add(this.GetBadge_UnitTests(this));

                if (this.Generator.DocumentAttributeCoverage)
                    Out.Add(this.GetBadge_AttributeCoverage(this));

                Out.Add(this.GetBadge_Assertions(this));

                // TODO: Add test coverage link
                }
            return Out;
            }


        /// <summary>
        /// Get the Covered badge
        /// </summary>
        public string GetBadge_Covered(GeneratedDocument MD, bool AsHtml = false)
            {
            return this.Meta.Coverage?.IsCovered == true
                ? MD.Badge(this.Generator.Language.Badge_Covered, "Yes", BadgeColor.BrightGreen, AsHtml)
                : MD.Badge(this.Generator.Language.Badge_Covered, "No", BadgeColor.Red, AsHtml);
            }

        /// <summary>
        /// Get the Coverage badge
        /// </summary>
        public string GetBadge_AttributeCoverage(GeneratedDocument MD, bool AsHtml = false)
            {
            return MD.Badge(this.Generator.Language.Badge_AttributeTests,
                $"{this.Meta.Coverage.AttributeCoverage}",
                this.Meta.Coverage.AttributeCoverage > 0u
                    ? BadgeColor.BrightGreen
                    : BadgeColor.LightGrey, AsHtml);
            }

        /// <summary>
        /// Get the Unit Tests badge
        /// </summary>
        public string GetBadge_UnitTests(GeneratedDocument MD, bool AsHtml = false)
            {
            return MD.Badge(this.Generator.Language.Badge_UnitTested, this.Meta.Coverage.MemberTraitFound
                ? "Yes"
                : "No", this.Meta.Coverage.MemberTraitFound
                ? BadgeColor.BrightGreen
                : BadgeColor.LightGrey, AsHtml);
            }

        /// <summary>
        /// Get the Assertions badge
        /// </summary>
        public string GetBadge_Assertions(GeneratedDocument MD, bool AsHtml = false)
            {
            return MD.Link(MD.GetRelativePath(this.Meta.CodeFilePath),
                MD.Badge(this.Generator.Language.Badge_Assertions, $"{this.Meta.Coverage.AssertionsMade}", this.Meta.Coverage.AssertionsMade > 0u
                    ? BadgeColor.BrightGreen
                    : BadgeColor.LightGrey, AsHtml), EscapeText: false, AsHtml: AsHtml);
            }

        #endregion
        }
    }