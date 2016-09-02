using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LCore.Extensions;

// ReSharper disable MemberCanBeProtected.Global

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Abstract class for generating markdown documents
    /// </summary>
    public abstract class GeneratedDocument : Markdown
        {
        /// <summary>
        /// The <see cref="SolutionMarkdownGenerator "/> that generated this document
        /// </summary>
        [NotNull]
        protected SolutionMarkdownGenerator Generator { get; }

        /// <summary>
        /// The path relative to the root repository folder that this markdown file will be saved
        /// </summary>
        public string FilePath { get; }

        /// <inheritdoc />
        public override void Line(string Line)
            {
            this.Generator.Stats.Lines++;

            base.Line(Line);
            }

        /// <inheritdoc />
        public override void Table(IEnumerable<IEnumerable<string>> Rows, bool IncludeHeader = true, L.Align[] Alignment = null, bool AsHtml = false,
            string TableWidth = "")
            {
            this.Generator.Stats.Tables++;

            base.Table(Rows, IncludeHeader, Alignment, AsHtml, TableWidth);
            }

        /// <inheritdoc />
        public override string Header(string Line, int Size = 1, bool AsHtml = false)
            {
            this.Generator.Stats.Headers++;

            return base.Header(Line, Size, AsHtml);
            }

        /// <inheritdoc />
        public override void HeaderUnderline(string Line, int Size = 1)
            {
            this.Generator.Stats.Headers++;

            base.HeaderUnderline(Line, Size);
            }

        /// <inheritdoc />
        public override string Link(string Url = "", string Text = "", string ReferenceText = "", bool TargetNewWindow = false,
            bool EscapeText = true, bool AsHtml = false)
            {
            this.Generator.Stats.Links++;

            return base.Link(Url, Text, ReferenceText, TargetNewWindow, EscapeText, AsHtml);
            }

        /// <inheritdoc />
        public override string Badge(string Left, string Right, string HexColor, bool AsHtml = false)
            {
            this.Generator.Stats.Badges++;

            return base.Badge(Left, Right, HexColor, AsHtml);
            }

        /// <summary>
        /// Retrieves the relative path from this markdown file to <paramref name="FullPath"/>
        /// </summary>
        public string GetRelativePath([CanBeNull] string FullPath)
            {
            this.Generator.Stats.LocalLinks++;

            if (FullPath == null)
                return "";

            if (string.IsNullOrEmpty(this.FilePath))
                return FullPath;


            var Uri1 = new Uri(FullPath);
            var Uri2 = new Uri(this.FilePath);

            var Out = Uri2.MakeRelativeUri(Uri1);

            return Out.ToString();
            }

        /// <summary>
        /// Pass a <paramref name="Generator"/> and <paramref name="FilePath"/> and <paramref name="Title"/> to create a generated document
        /// </summary>
        protected GeneratedDocument(SolutionMarkdownGenerator Generator, string FilePath, string Title) : base(Title)
            {
            this.Generator = Generator;
            this.FilePath = FilePath;
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

        /// <summary>
        /// Returns the full Url for a document once it's deployed
        /// </summary>
        /// <returns></returns>
        public string GetLiveUrl()
            {
            string FullPath = this.FilePath;
            string RootSolution = L.Ref.GetSolutionRootPath();
            string RootGitHub = $"{this.Generator.RootUrl}/blob/master";

            string Out = FullPath.Replace(RootSolution, RootGitHub)
                .ReplaceAll("\\", "/");

            return Out;
            }
        }
    }