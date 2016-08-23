using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    public class GitHubMarkdown_Type : GitHubMarkdown
        {
        public ICodeComment Comments { get; }
        public TypeCoverage Coverage { get; }

        public GitHubMarkdown_Type(Type Member, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Comments = Member.GetComments();
            this.Coverage = new TypeCoverage(Member);
            }
        }
    }