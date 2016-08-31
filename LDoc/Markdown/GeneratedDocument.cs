using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Abstract class for generating markdown documents
    /// </summary>
    public abstract class GeneratedDocument : GitHubMarkdown
        {
        /// <summary>
        /// The <see cref="SolutionMarkdownGenerator "/> that generated this document
        /// </summary>
        [NotNull]
        protected new SolutionMarkdownGenerator Generator { get; }

        /// <summary>
        /// Pass a <paramref name="Generator"/> and <paramref name="FilePath"/> and <paramref name="Title"/> to create a generated document
        /// </summary>
        protected GeneratedDocument(SolutionMarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Generator = Generator;
            }

        /// <summary>
        /// Generate the document
        /// </summary>
        public void Generate()
            {
            this.Generator.Stats.MarkdownDocuments++;

            this.GenerateDocument();
            }

        /// <summary>
        /// Generate the document
        /// </summary>
        protected abstract void GenerateDocument();
        }
    }