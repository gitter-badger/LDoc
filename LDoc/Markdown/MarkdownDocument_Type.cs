using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LUnit;

// ReSharper disable SuggestBaseTypeForParameter

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for a Type.
    /// </summary>
    public class MarkdownDocument_Type : GeneratedDocument
        {
        /// <summary>
        /// MetaData for the type
        /// </summary>
        public CodeCoverageMetaData TypeMeta { get; }

        /// <summary>
        /// All member markdown defined under the current Type
        /// </summary>
        public Dictionary<MemberInfo, MarkdownDocument_Member> MemberMarkdown { get; } =
            new Dictionary<MemberInfo, MarkdownDocument_Member>();

        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public MarkdownDocument_Type(Type Type, SolutionMarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            this.TypeMeta = Type.GatherCodeCoverageMetaData(Generator.CustomCommentTags);

            Generator.GetTypeMemberMarkdown((Type) this.TypeMeta?.Member).Each(
                MD => this.MemberMarkdown.Add(MD.Key, MD.Value));
            }

        /// <summary>
        /// Generate the document.
        /// </summary>
        public override void Generate()
            {
            this.Generator.WriteHeader(this);

            this.Line(
                this.Link(
                    this.GetRelativePath(this.Generator.MarkdownPath_Assembly(this.TypeMeta.Member.GetAssembly())),
                    this.Generator.Language.LinkText_Up, EscapeText: false));

            this.Header($"{((Type) this.TypeMeta.Member).GetGenericName()}", Size: 3);
            this.Line("");
            this.Line(
                this.Generator.GetBadges_Info(this, new TypeCoverage((Type) this.TypeMeta.Member),
                    this.TypeMeta.Comments).JoinLines(" "));
            this.Line("");
            this.Line(
                this.Generator.GetBadges_Coverage(this, new TypeCoverage((Type) this.TypeMeta.Member),
                    this.TypeMeta.Comments).JoinLines(" "));
            this.Line("");
            string TypePath = this.TypeMeta.CodeFilePath;

            if (!string.IsNullOrEmpty(TypePath))
                {
                this.Line(this.Link($"{this.GetRelativePath(TypePath)}#L{this.TypeMeta.CodeLineNumber}",
                    this.Generator.Language.LinkText_ViewSource, EscapeText: false));
                }

            if (!string.IsNullOrEmpty(this.TypeMeta.Comments?.Summary))
                {
                this.Header(this.Generator.Language.Header_Summary, Size: 6);
                this.Line(this.TypeMeta.Comments?.Summary);
                }

            // TODO display constructors 
            // TODO display attributes
            // TODO display interfaces 
            // TODO display constructors
            // TODO display subtypes

            Dictionary<string, List<KeyValuePair<MemberInfo, MarkdownDocument_Member>>> MemberGroups =
                this.MemberMarkdown.Group(Member => Member.Key.GetMemberDetails()?.ToString());

            MemberGroups.Each(Group =>
                {
                uint Documented = 0;
                uint DocumentedTotal = 0;

                uint Covered = 0;
                uint CoveredTotal = 0;

                uint LinesTotal = 0;

                uint TotalTodos = 0;
                uint TotalBugs = 0;
                uint TotalNotImplemented = 0;

                var Body = new List<string[]>();

                Group.Value.Each(Member =>
                    {
                    var MD = Member.Value;
                    var Meta = MD.Meta;

                    LinesTotal += Meta.CodeLineCount ?? 0u;

                    Covered += Meta.Coverage?.IsCovered == true
                        ? 1u
                        : 0u;
                    CoveredTotal += 1u;

                    Documented += Meta.Comments == null
                        ? 0u
                        : 1u;
                    DocumentedTotal += 1u;

                    TotalTodos += (uint) Meta.CommentTODO.Length;
                    TotalBugs += (uint) Meta.CommentBUG.Length;
                    TotalNotImplemented += (uint) Meta.NotImplemented.Length;
                    // TODO total for custom tags

                    Body.Add(new[]
                        {
                        this.Bold(this.Link(this.GetRelativePath(this.Generator.FindMarkdown(Member.Key).FilePath), Member.Key.Name, AsHtml: true), AsHtml: true),
                        MD.GetBadge_Todos(this, AsHtml: true) + " " +
                        MD.GetBadge_Bugs(this, AsHtml: true) + " " +
                        MD.GetBadge_NotImplemented(this, AsHtml: true) + " " +
                        MD.GetBadge_CustomTags(this, AsHtml: true).JoinLines(" "),
                        MD.GetBadge_CodeLines(this, AsHtml: true),
                        MD.GetBadge_Documented(this, AsHtml: true),
                        MD.GetBadge_Covered(this, AsHtml: true)
                        });
                    Body.Add(new[]
                        {
                        MD.GetSignature(this, AsHtml: true)
                        });
                    });

                int CoveredPercent = Covered.PercentageOf(CoveredTotal);
                int DocumentedPercent = Documented.PercentageOf(DocumentedTotal);

                var Header = new[]
                    {
                    new[]
                        {
                        $"{Group.Key.Pluralize()} ({Group.Value.Count})",
                        this.GetBadge_TotalTodos(TotalTodos, AsHtml: true) +
                        (TotalBugs > 0
                            ? this.Badge(this.Generator.Language.Badge_BUGs, $"{TotalBugs}", BadgeColor.Red, AsHtml: true)
                            : "") +
                        (TotalNotImplemented > 0
                            ? this.Badge(this.Generator.Language.Badge_NotImplemented, $"{TotalNotImplemented}", BadgeColor.Orange, AsHtml: true)
                            : "")
                        // TODO total for custom tags
                        ,
                        this.Badge($"Total {this.Generator.Language.Header_CodeLines}", $"{LinesTotal}", LinesTotal == 0
                            ? BadgeColor.Red
                            : BadgeColor.Blue, AsHtml: true),
                        this.Badge($"Total {this.Generator.Language.Header_Documentation}", $"{DocumentedPercent}%", this.Generator.GetColorByPercentage(DocumentedPercent), AsHtml: true),
                        this.Badge($"Total {this.Generator.Language.Header_Coverage}", $"{CoveredPercent}%", this.Generator.GetColorByPercentage(CoveredPercent), AsHtml: true)
                        }
                    };

                this.Table(Header.Add(Body), AsHtml: true, TableWidth: "850px");
                });

            this.Generator.WriteFooter(this);
            }

        private string GetBadge_TotalTodos(uint TotalTodos, bool AsHtml = false)
            {
            return TotalTodos > 0
                ? this.Badge(this.Generator.Language.Badge_TODOs, $"{TotalTodos}", BadgeColor.Orange, AsHtml)
                : "";
            }
        }
    }