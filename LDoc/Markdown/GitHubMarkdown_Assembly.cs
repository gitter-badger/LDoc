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
    /// Generates markdown for an Assembly.
    /// </summary>
    public class GitHubMarkdown_Assembly : GitHubMarkdown
        {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; }

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
            this.Assembly = Assembly;
            this.Comments = null; //Assembly.GetComments();
            this.Coverage = new AssemblyCoverage(Assembly);

            this.Generate();
            }

        private void Generate()
            {
            var MarkdownGenerator = this.Generator;
            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                var Coverage = new AssemblyCoverage(this.Assembly);
                ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Root), MarkdownGenerator.Language.LinkText_Home));

                this.Header($"{this.Assembly.GetName().Name}", Size: 2);

                // ReSharper disable once ExpressionIsAlwaysNull
                this.Line(MarkdownGenerator.GetBadges(this, Coverage, Comments).JoinLines(" "));

                // TODO sort by class namespace

                MarkdownGenerator.GetAssemblyTypeMarkdown(this.Assembly).Each(
                    MD2 => this.Line($" - {this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Type(MD2.Key)), MD2.Key.Name)}"));

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }