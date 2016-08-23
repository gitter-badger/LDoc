using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    public class GitHubMarkdown_Member : GitHubMarkdown
        {
        public Dictionary<MemberInfo, Tuple<ICodeComment, MethodCoverage>> Members { get; } = new Dictionary<MemberInfo, Tuple<ICodeComment, MethodCoverage>>();

        public GitHubMarkdown_Member(MemberInfo[] Members, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            Members.Each(Member =>
                {
                    this.Members.Add(Member,
                        new Tuple<ICodeComment, MethodCoverage>(
                            Member.GetComments(),
                            Member is MethodInfo ? new MethodCoverage((MethodInfo)Member) : null));
                });
            }
        }
    }