using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for a Member.
    /// </summary>
    public class GitHubMarkdown_MemberGroup : GitHubMarkdown
        {
        /// <summary>
        /// Members and corresponding comments and coverage
        /// </summary>
        public Dictionary<MemberInfo, CodeCoverageMetaData> Members { get; }
            = new Dictionary<MemberInfo, CodeCoverageMetaData>();

        /// <summary>
        /// Create a new Member Markdown file.
        /// </summary>
        public GitHubMarkdown_MemberGroup(MemberInfo[] Members, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            Members.Each(Member =>
                {
                    this.Members.Add(Member, new CodeCoverageMetaData(Member, Generator.CustomCommentTags));
                });

            this.Generate();
            }

        private void Generate()
            {
            if (this.Members.Keys.Count == 1)
                this.Generate_Single();
            else
                this.Generate_Multi();
            }

        private void Generate_Single()
            {
            var MarkdownGenerator = this.Generator;

            var Member = this.Members.Keys.First();
            if (MarkdownGenerator != null && Member != null)
                {
                MarkdownGenerator.WriteHeader(this);
                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Type(Member.DeclaringType)),
                    MarkdownGenerator.Language.LinkText_Up));

                this.Header($"{Member.DeclaringType?.Name}", Size: 3);

                string TypePath = Member.DeclaringType.FindClassFile();

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link(this.GetRelativePath(TypePath), MarkdownGenerator.Language.LinkText_ViewSource));
                    }


                this.Header(Member.Name);

                if (Member is MethodInfo)
                    {
                    var Meta = this.Members[Member];

                    var Method = (MethodInfo)Member;

                    string Static = Method.IsStatic
                        ? "Static "
                        : "Instance";

                    string StaticLower = Method.IsStatic
                        ? " static"
                        : "";

                    string Parameters = Method.GetParameters()
                        .Convert(Param => $"{MarkdownGenerator.LinkToType(this, Param.ParameterType)} {Param.Name}")
                        .Combine(", ");

                    this.Header($"{Static}Method", Size: 4);


                    this.Header(
                        $"public{StaticLower} {MarkdownGenerator.LinkToType(this, Method.ReturnType)} {Member.Name}({Parameters});",
                        Size: 6);

                    this.Line("");
                    this.Line(MarkdownGenerator.GetBadges_Info(this, Meta.Coverage, Meta.Comments).JoinLines(" "));
                    this.Line("");
                    this.Line(MarkdownGenerator.GetBadges_Coverage(this, Meta.Coverage, Meta.Comments).JoinLines(" "));

                    // TODO: Add test coverage link

                    if (Meta.Comments?.Summary != null)
                        {
                        this.Header(MarkdownGenerator.Language.Header_Summary, Size: 5);
                        this.Line(this.Generator?.FormatComment(Meta.Comments?.Summary));
                        }

                    if (Method.GetParameters().Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodParameters, Size: 6);

                        var Table = new List<string[]>
                            {
                            new[]
                                {
                                MarkdownGenerator.Language.TableHeaderText_MethodParameter,
                                MarkdownGenerator.Language.TableHeaderText_Optional,
                                MarkdownGenerator.Language.TableHeaderText_Type,
                                MarkdownGenerator.Language.TableHeaderText_Description
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
                                MarkdownGenerator.LinkToType(this, Param.ParameterType),
                                Meta.Comments?.Parameters.GetAt(ParamIndex)?.Obj2
                                });
                            });

                        this.Table(Table);
                        }

                    this.Header(MarkdownGenerator.Language.Header_MethodReturns, Size: 4);

                    this.Header(MarkdownGenerator.LinkToType(this, Method.ReturnType), Size: 6);

                    this.Line(Meta.Comments?.Returns);

                    if (Meta.Comments?.Examples.Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodExamples, Size: 4);
                        Meta.Comments.Examples.Each(Example => this.Code(new[] { Example }));
                        }

                    if (Meta.Comments?.Permissions.Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodPermissions, Size: 4);
                        Meta.Comments.Permissions.Each(Permission => this.Line($"{Permission.Obj1} {Permission.Obj2}"));
                        }

                    if (Meta.Comments?.Exceptions.Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodExceptions, Size: 4);
                        Meta.Comments.Exceptions.Each(Exception => this.Line($"{Exception.Obj1} {Exception.Obj2}"));
                        }
                    }

                MarkdownGenerator.WriteFooter(this);

                }
            }

        private void Generate_Multi()
            {


            }
        }
    }