using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
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
                MD =>
                    {
                    this.TotalTodos += (uint) MD.Value.Meta.CommentTODO.Length;
                    this.TotalBugs += (uint) MD.Value.Meta.CommentBUG.Length;
                    this.TotalNotImplemented += (uint) MD.Value.Meta.NotImplemented.Length;

                    this.MemberMarkdown.Add(MD.Key, MD.Value);
                    });
            }

        /// <summary>
        /// Total number of Todos found within the current type
        /// </summary>
        public uint TotalTodos { get; protected set; }

        /// <summary>
        /// Total number of bugs found within the current type
        /// </summary>
        public uint TotalBugs { get; protected set; }

        /// <summary>
        /// Total number of not implemented exceptions found within the current type
        /// </summary>
        public uint TotalNotImplemented { get; protected set; }


        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.TypeMarkdownDocuments++;

            this.Generator.WriteHeader(this);

            this.Line(
                this.Link(
                    this.GetRelativePath(this.Generator.MarkdownPath_Assembly(this.TypeMeta.Member.GetAssembly())),
                    this.Generator.Language.LinkText_Up, EscapeText: false));

            this.Line(this.Header($"{((Type) this.TypeMeta.Member).GetGenericName()}", Size: 3));
            this.Line("");
            this.Line(this.GetBadges_Info(this).JoinLines(" "));
            this.Line("");
            this.Line(this.GetBadges_Coverage(this).JoinLines(" "));
            this.Line("");
            string TypePath = this.TypeMeta.CodeFilePath;

            if (!string.IsNullOrEmpty(TypePath))
                {
                this.Line(this.Link($"{this.GetRelativePath(TypePath)}#L{this.TypeMeta.CodeLineNumber}",
                    this.Generator.Language.LinkText_ViewSource, EscapeText: false));
                }

            if (!string.IsNullOrEmpty(this.TypeMeta.Comments?.Summary))
                {
                this.Line(this.Header(this.Generator.Language.Header_Summary, Size: 6));
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

                var Body = new List<string[]>();

                uint GroupTotalTodo = 0;
                uint GroupTotalBugs = 0;
                uint GroupTotalNotImplemented = 0;

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

                    GroupTotalTodo += (uint) Meta.CommentTODO.Length;
                    GroupTotalBugs += (uint) Meta.CommentBUG.Length;
                    GroupTotalNotImplemented += (uint) Meta.NotImplemented.Length;
                    // TODO total for custom tags

                    Body.Add(new[]
                        {
                        this.Header(this.Bold(this.Link(this.GetRelativePath(this.Generator.FindMarkdown(Member.Key).FilePath),
                            Member.Key.Name, AsHtml: true), AsHtml: true), Size: 4, AsHtml: true),
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
                        $"{this.Header(MD.GetSignature(this, AsHtml: true), Size: 6, AsHtml: true)}\r\n"
                        });
                    });

                int CoveredPercent = Covered.PercentageOf(CoveredTotal);
                int DocumentedPercent = Documented.PercentageOf(DocumentedTotal);

                var Header = new[]
                    {
                    new[]
                        {
                        $"{Group.Key.Pluralize()} ({Group.Value.Count})",
                        this.GetBadge_TotalTodos(this, GroupTotalTodo, AsHtml: true) +
                        this.GetBadge_TotalBugs(this, GroupTotalBugs, AsHtml: true) +
                        this.GetBadge_TotalNotImplemented(this, GroupTotalNotImplemented, AsHtml: true)
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


        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        [CanBeNull]
        public virtual List<string> GetBadges_Info([NotNull] GeneratedDocument MD)
            {
            var Type = (Type) this.TypeMeta.Member;

            if (Type != null)
                {
                var Out = new List<string>();

                string TypeDescription =
                    Type.IsStatic()
                        ? "Static Class"
                        : Type.IsAbstract
                            ? "Abstract Class"
                            : Type.IsEnum
                                ? "Enum "
                                : Type.IsInterface
                                    ? "Interface"
                                    : "Object Class";

                List<KeyValuePair<MemberInfo, MarkdownDocument_Member>> Members = this.Generator.GetTypeMemberMarkdown(Type);

                uint TotalDocumentable = 0;
                uint Documented = 0;

                // TODO: add total members
                // TODO: add total lines of code (non 'empty')
                // TODO: add total extension methods
                // TODO: add total todo count
                // TODO: add total bug count
                // TODO: add total not implemented count

                Members.Each(Member =>
                    {
                    TotalDocumentable++;

                    var MemberComments = Member.Key.GetComments();

                    if (MemberComments != null)
                        Documented++;
                    });


                Out.Add(MD.Badge(this.Generator.Language.Badge_Type, TypeDescription, BadgeColor.Blue));

                if (TotalDocumentable > 0)
                    {
                    int DocumentedPercent = Documented.PercentageOf(TotalDocumentable);
                    Out.Add(MD.Badge(this.Generator.Language.Badge_Documented, $"{DocumentedPercent}%",
                        this.Generator.GetColorByPercentage(DocumentedPercent)));
                    }

                return Out;
                }

            return null;
            }

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        [CanBeNull]
        public virtual List<string> GetBadges_Coverage([NotNull] GeneratedDocument MD)
            {
            var Type = (Type) this.TypeMeta.Member;

            if (Type != null)
                {
                var Out = new List<string>();

                List<KeyValuePair<MemberInfo, MarkdownDocument_Member>> Members = this.Generator.GetTypeMemberMarkdown(Type);

                uint TotalCoverable = 0;
                uint Covered = 0;

                Members.Each(Member =>
                    {
                    if (Member.Key is MethodInfo)
                        {
                        TotalCoverable++;

                        var MemberCoverage = this.TypeMeta.Coverage;

                        if (MemberCoverage?.IsCovered == true)
                            Covered++;
                        }
                    });


                if (TotalCoverable > 0)
                    {
                    int CoveredPercent = Covered.PercentageOf(TotalCoverable);
                    Out.Add(MD.Badge(this.Generator.Language.Badge_Covered, $"{CoveredPercent}%",
                        this.Generator.GetColorByPercentage(CoveredPercent)));
                    }

                return Out;
                }

            return null;
            }


        /// <summary>
        /// Get a badge representing total Not Implemented Exceptions declared
        /// </summary>
        public string GetBadge_TotalNotImplemented(GeneratedDocument MD, uint GroupTotalNotImplemented, bool AsHtml)
            {
            return GroupTotalNotImplemented > 0
                ? MD.Badge(this.Generator.Language.Badge_NotImplemented, $"{GroupTotalNotImplemented}", BadgeColor.Orange, AsHtml)
                : "";
            }

        /// <summary>
        /// Get a badge representing total bugs declared
        /// </summary>
        public string GetBadge_TotalBugs(GeneratedDocument MD, uint GroupTotalBugs, bool AsHtml)
            {
            return GroupTotalBugs > 0
                ? MD.Badge(this.Generator.Language.Badge_BUGs, $"{GroupTotalBugs}", BadgeColor.Red, AsHtml)
                : "";
            }

        /// <summary>
        /// Get a badge representing total todos declared
        /// </summary>
        public string GetBadge_TotalTodos(GeneratedDocument MD, uint GroupTotalTodo, bool AsHtml = false)
            {
            return GroupTotalTodo > 0
                ? MD.Badge(this.Generator.Language.Badge_TODOs, $"{this.TotalTodos}", BadgeColor.Orange, AsHtml)
                : "";
            }
        }
    }