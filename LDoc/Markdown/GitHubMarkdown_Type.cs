using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for a Type.
    /// </summary>
    public class GitHubMarkdown_Type : GitHubMarkdown
        {
        /// <summary>
        /// Type comments
        /// </summary>
        public ICodeComment Comments { get; }

        /// <summary>
        /// Type coverage
        /// </summary>
        public TypeCoverage Coverage { get; }

        /// <summary>
        /// Create a new Type Markdown file.
        /// </summary>
        public GitHubMarkdown_Type(Type Member, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Comments = Member.GetComments();
            this.Coverage = new TypeCoverage(Member);
            }
        }
    }