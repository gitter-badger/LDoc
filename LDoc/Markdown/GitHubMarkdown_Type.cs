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
    public class GitHubMarkdown_Type : GitHubMarkdown
        {
        /// <summary>
        /// MetaData for the type
        /// </summary>
        public CodeCoverageMetaData TypeMeta { get; }

        /// <summary>
        /// All member markdown defined under the current Type
        /// </summary>
        public Dictionary<MemberInfo, CodeCoverageMetaData> MemberMarkdown { get; } =
            new Dictionary<MemberInfo, CodeCoverageMetaData>();


        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public GitHubMarkdown_Type(Type Type, MarkdownGenerator Generator, string FilePath, string Title)
            : base(Generator, FilePath, Title)
            {
            this.TypeMeta = Type.GatherCodeCoverageMetaData(Generator.CustomCommentTags);

            Generator.GetTypeMemberMarkdown((Type)this.TypeMeta?.Member).Each(
                MD => MD.Value.Members.Each(
                    Member => this.MemberMarkdown.Add(Member.Key, Member.Value)));

            this.Generate();
            }


        private void Generate()
            {
            var MarkdownGenerator = this.Generator;

            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                this.Line(
                    this.Link(
                        this.GetRelativePath(MarkdownGenerator.MarkdownPath_Assembly(this.TypeMeta.Member.GetAssembly())),
                        MarkdownGenerator.Language.LinkText_Up, EscapeText: false));

                this.Header($"{((Type)this.TypeMeta.Member).GetGenericName()}", Size: 3);
                this.Line("");
                this.Line(
                    MarkdownGenerator.GetBadges_Info(this, new TypeCoverage((Type)this.TypeMeta.Member),
                        this.TypeMeta.Comments).JoinLines(" "));
                this.Line("");
                this.Line(
                    MarkdownGenerator.GetBadges_Coverage(this, new TypeCoverage((Type)this.TypeMeta.Member),
                        this.TypeMeta.Comments).JoinLines(" "));
                this.Line("");
                string TypePath = this.TypeMeta.CodeFilePath;

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link($"{this.GetRelativePath(TypePath)}#L{this.TypeMeta.CodeLineNumber}",
                        MarkdownGenerator.Language.LinkText_ViewSource, EscapeText: false));
                    }

                if (!string.IsNullOrEmpty(this.TypeMeta.Comments?.Summary))
                    {
                    this.Header(MarkdownGenerator.Language.Header_Summary, Size: 6);
                    this.Line(this.TypeMeta.Comments?.Summary);
                    }

                Dictionary<string, List<KeyValuePair<MemberInfo, CodeCoverageMetaData>>> MemberGroups =
                    this.MemberMarkdown.Group(Member => Member.Key.GetMemberDetails().ToString());

                MemberGroups.Each(Group =>
                    {
                        uint Documented = 0;
                        uint DocumentedTotal = 0;

                        uint Covered = 0;
                        uint CoveredTotal = 0;

                        uint LinesTotal = 0;

                        string[][] Body = Group.Value.Convert(Member =>
                            {

                                LinesTotal += Member.Value.CodeLineCount ?? 0u;

                                Covered += Member.Value.Coverage?.IsCovered == true ? 1u : 0u;
                                CoveredTotal += 1u;

                                Documented += Member.Value.Comments == null ? 0u : 1u;
                                DocumentedTotal += 1u;

                                return new[]
                                    {
                                this.Bold(this.Link(this.GetRelativePath(MarkdownGenerator.FindMarkdown(Member.Key).FilePath), Member.Key.Name)),

                                this.Link(this.GetRelativePath(Member.Value.CodeFilePath),
                                    this.Badge(MarkdownGenerator.Language.Badge_LinesOfCode, $"{Member.Value.CodeLineCount}", BadgeColor.Blue), EscapeText:false),

                                Member.Value.Comments != null
                                    ? this.Badge(MarkdownGenerator.Language.Badge_Documented, "Yes", BadgeColor.BrightGreen)
                                    : this.Badge(MarkdownGenerator.Language.Badge_Documented, "No", BadgeColor.Red),

                                Member.Value.Coverage?.IsCovered == true
                                    ? this.Badge(MarkdownGenerator.Language.Badge_Covered, "Yes", BadgeColor.BrightGreen)
                                    : this.Badge(MarkdownGenerator.Language.Badge_Covered, "No", BadgeColor.Red)
                                };
                            }).Array();

                        int CoveredPercent = Covered.PercentageOf(CoveredTotal);
                        int DocumentedPercent = Documented.PercentageOf(DocumentedTotal);

                        var Header = new[]
                                {
                            new[]
                                {
                                $"{Group.Key.Pluralize()} ({ Group.Value.Count})",
                                this.Badge($"Total {MarkdownGenerator.Language.Header_CodeLines}", $"{LinesTotal}", BadgeColor.Blue),
                                this.Badge($"Total {MarkdownGenerator.Language.Header_Documentation}",$"{DocumentedPercent}%", MarkdownGenerator.GetColorByPercentage(DocumentedPercent)),
                                this.Badge($"Total {MarkdownGenerator.Language.Header_Coverage}",$"{CoveredPercent}%", MarkdownGenerator.GetColorByPercentage(CoveredPercent))
                                }
                            };

                        this.Table(Header.Add(Body));
                    });

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }
