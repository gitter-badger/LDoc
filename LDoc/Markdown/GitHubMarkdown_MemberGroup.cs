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
            }
        }
    }