using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    public class GitHubMarkdown_Assembly : GitHubMarkdown
        {
        public ICodeComment Comments { get; }
        public AssemblyCoverage Coverage { get; }

        public GitHubMarkdown_Assembly(Assembly Assembly, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Comments = null;//Assembly.GetComments();
            this.Coverage = new AssemblyCoverage(Assembly);
            }
        }
    }