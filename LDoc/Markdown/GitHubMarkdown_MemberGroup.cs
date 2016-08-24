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
    /// Generates markdown for a Member.
    /// </summary>
    public class GitHubMarkdown_MemberGroup : GitHubMarkdown
        {
        /// <summary>
        /// Members and corresponding comments and coverage
        /// </summary>
        public Dictionary<MemberInfo, Tuple<ICodeComment, MethodCoverage>> Members { get; }
            = new Dictionary<MemberInfo, Tuple<ICodeComment, MethodCoverage>>();

        /// <summary>
        /// Create a new Member Markdown file.
        /// </summary>
        public GitHubMarkdown_MemberGroup(MemberInfo[] Members, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            Members.Each(Member =>
                {
                    this.Members.Add(Member,
                        new Tuple<ICodeComment, MethodCoverage>(
                            Member.GetComments(),
                            Member is MethodInfo ? new MethodCoverage((MethodInfo)Member) : null));
                });

            this.Generate();
            }

        private void Generate()
            {
            var MarkdownGenerator = this.Generator;

            var Member = this.Members.Keys.First();
            if (MarkdownGenerator != null && Member != null)
                {
                MarkdownGenerator.WriteHeader(this);
                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Type(Member.DeclaringType)), MarkdownGenerator.Language.LinkText_Up));

                this.Header($"{Member.DeclaringType?.Name}", Size: 3);

                string TypePath = Member.DeclaringType.FindClassFile();

                if (!string.IsNullOrEmpty(TypePath))
                    {
                    this.Line(this.Link(this.GetRelativePath(TypePath), MarkdownGenerator.Language.LinkText_ViewSource));
                    }


                this.Header(Member.Name);

                if (Member is MethodInfo)
                    {
                    var Method = (MethodInfo)Member;
                    var Coverage = new MethodCoverage(Method);
                    var Comments = Method.GetComments();

                    string Static = Method.IsStatic
                        ? "Static "
                        : "Instance";

                    string StaticLower = Method.IsStatic
                        ? " static"
                        : "";

                    string ReturnType = this.Link(MarkdownGenerator.GetTypeLink(this, Method.ReturnType), $"{Method.ReturnType.GetGenericName()}");


                    string Parameters = Method.GetParameters()
                        .Convert(Param => $"{this.Link(MarkdownGenerator.GetTypeLink(this, Param.ParameterType), $"{Param.ParameterType.GetGenericName()}")} {Param.Name}")
                        .Combine(", ");

                    List<string> Badges = MarkdownGenerator.GetBadges(this, Coverage, Comments);

                    this.Line("");
                    this.Line(Badges.JoinLines(" "));
                    this.Line("");

                    this.Header($"{Static}Method", Size: 4);


                    this.Header($"public{StaticLower} {ReturnType} {Member.Name}({Parameters});", Size: 6);

                    this.Header(MarkdownGenerator.Language.Header_Summary, Size: 6);
                    this.Line(Comments?.Summary);

                    if (Method.GetParameters().Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodParameters, Size: 6);

                        var Table = new List<string[]>
                            {
                            new[] {MarkdownGenerator.Language.TableHeaderText_MethodParameter,
                            MarkdownGenerator.Language.TableHeaderText_Optional,
                            MarkdownGenerator.Language.TableHeaderText_Type,
                            MarkdownGenerator.Language.TableHeaderText_Description}
                            };

                        Method.GetParameters().Each((ParamIndex, Param) =>
                            {
                                Table.Add(new[]
                                {
                                Param.Name,
                                Param.IsOptional
                                    ? "Yes"
                                    : "No",
                                this.Link(MarkdownGenerator.GetTypeLink(this, Param.ParameterType), Param.ParameterType.GetGenericName()),
                                Comments?.Parameters.GetAt(ParamIndex)?.Obj2
                                });
                            });

                        this.Table(Table);
                        }

                    this.Header(MarkdownGenerator.Language.Header_MethodReturns, Size: 4);

                    this.Header(this.Link(MarkdownGenerator.GetTypeLink(this, Method.ReturnType), Method.ReturnType.GetGenericName()), Size: 6);

                    this.Line(Comments?.Returns);

                    if (Comments?.Examples.Length > 0)
                        {
                        this.Header(MarkdownGenerator.Language.Header_MethodExamples, Size: 4);
                        Comments.Examples.Each(Example => this.Code(new[] { Example }));
                        }

                    // TODO: Add test coverage link

                    // TODO: Add exception details

                    // TODO: Add permission details
                    }

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }