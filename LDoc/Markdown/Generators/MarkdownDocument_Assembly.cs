using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;

// ReSharper disable ExpressionIsAlwaysNull

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for an Assembly.
    /// </summary>
    public class MarkdownDocument_Assembly : GeneratedDocument
        {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Assembly coverage
        /// </summary>
        public AssemblyCoverage Coverage { get; }

        /// <summary>
        /// All types under the <see cref="Assembly"/>
        /// </summary>
        public List<KeyValuePair<Type, MarkdownDocument_Type>> TypesCovered { get; }

        /// <summary>
        /// Total number of todo items within the type's members
        /// </summary>
        public uint TotalTodos { get; }

        /// <summary>
        /// Total number of bug items within the type's members
        /// </summary>
        public uint TotalBugs { get; }

        /// <summary>
        /// Total number of not implemented items within the type's members
        /// </summary>
        public uint TotalNotImplemented { get; }


        /// <summary>
        /// Create a new Assembly Markdown file.
        /// </summary>
        public MarkdownDocument_Assembly(Assembly Assembly, SolutionMarkdownGenerator Generator, string Title)
            : base(Generator, Title)
            {
            this.Assembly = Assembly;
            this.Coverage = new AssemblyCoverage(Assembly);

            this.TypesCovered = this.Generator.GetAssemblyTypeMarkdown(Assembly);

            this.TotalTodos = this.TypesCovered.Sum(Type => Type.Value.TotalTodos);
            this.TotalBugs = this.TypesCovered.Sum(Type => Type.Value.TotalBugs);
            this.TotalNotImplemented = this.TypesCovered.Sum(Type => Type.Value.TotalNotImplemented);
            }

        /// <inheritdoc />
        protected override string FileName => $"{this.Assembly.GetName().Name.CleanFileName()}.md";

        /// <inheritdoc />
        protected override string FilePath => this.Generator.MarkdownPath_AssemblyRoot(this.Assembly);

        /// <summary>
        /// Generate the markdown document
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.AssemblyMarkdownDocuments++;

            this.Generator.WriteHeader(this);

            ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

            this.Line(this.Link(this.GetRelativePath(this.Generator.Markdown_Root.FullPath), this.Generator.Language.LinkText_Home));

            this.Line(this.Header($"{this.Assembly.GetName().Name}", Size: 2));

            this.Line(this.GetBadges_Info(this, this.Coverage, Comments).JoinLines(" "));
            this.Line(this.GetBadges_Coverage(this, this.Coverage, Comments).JoinLines(" "));

            List<KeyValuePair<Type, MarkdownDocument_Type>> Types = this.Generator.GetAssemblyTypeMarkdown(this.Assembly);

            Dictionary<string, List<KeyValuePair<Type, MarkdownDocument_Type>>> NamespaceTypes = Types.Group(Type => Type.Key.Namespace);

            List<string> Namespaces = NamespaceTypes.Keys.List();

            Namespaces.Sort();

            this.GetBadges_Info(this, this.Coverage, Comments);
            this.GetBadges_Coverage(this, this.Coverage, Comments);

            Namespaces.Each(Namespace =>
                {
                    this.HeaderUnderline(Namespace, Size: 2);

                    List<KeyValuePair<Type, MarkdownDocument_Type>> NamespaceTypeMarkdown = NamespaceTypes[Namespace];

                    NamespaceTypeMarkdown.Sort(Type => Type.Key.Name);

                    // TODO namespace badges

                    NamespaceTypeMarkdown.Each(Type =>
                        {
                            this.Line(this.Header(this.Link(this.GetRelativePath(Type.Value.FullPath), Type.Key.GetGenericName()), Size: 4));

                            Type.Value.GetBadges_Info(Type.Value);
                            Type.Value.GetBadges_Coverage(Type.Value);
                        });
                });

            this.Generator.WriteFooter(this);
            }

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Info([NotNull] GeneratedDocument MD, [CanBeNull] AssemblyCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var Out = new List<string>();

            Out.Add(this.GetBadge_FrameworkVersion(MD));

            // TODO: add output file badge
            // TODO: add file size badge

            // TODO: add total classes
            // TODO: add total members
            // TODO: add total lines of code (non 'empty')
            // TODO: add total extension methods
            // TODO: add total todo count
            // TODO: add total bug count
            // TODO: add total not implemented count

            return Out;
            }

        /// <summary>
        /// Get the framework version badge
        /// </summary>
        public virtual string GetBadge_FrameworkVersion(GeneratedDocument MD)
            {
            return MD.Badge(this.Generator.Language.Badge_Framework,
                            $"Version {this.Assembly.ImageRuntimeVersion}",
                            BadgeColor.Blue);
            }

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Coverage([NotNull] GeneratedDocument MD, [CanBeNull] AssemblyCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Out = new List<string>();



            return Out;
            }
        }
    }