using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for an Assembly.
    /// </summary>
    public class GitHubMarkdown_Assembly : GitHubMarkdown
        {
        /// <summary>
        /// Assembly comments
        /// </summary>
        public ICodeComment Comments { get; }

        /// <summary>
        /// Assembly coverage
        /// </summary>
        public AssemblyCoverage Coverage { get; }


        /// <summary>
        /// Create a new Assembly Markdown file.
        /// </summary>
        public GitHubMarkdown_Assembly(Assembly Assembly, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Comments = null; //Assembly.GetComments();
            this.Coverage = new AssemblyCoverage(Assembly);
            }
        }
    }