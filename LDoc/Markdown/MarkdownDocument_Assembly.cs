using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        /// Create a new Assembly Markdown file.
        /// </summary>
        public MarkdownDocument_Assembly(Assembly Assembly, SolutionMarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Assembly = Assembly;
            this.Coverage = new AssemblyCoverage(Assembly);
            }

        /// <summary>
        /// Generate the markdown document
        /// </summary>
        public override void Generate()
            {
            this.Generator.WriteHeader(this);

            ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

            this.Line(this.Link(this.GetRelativePath(this.Generator.MarkdownPath_Root), this.Generator.Language.LinkText_Home));

            this.Header($"{this.Assembly.GetName().Name}", Size: 2);

            this.Line(this.Generator.GetBadges_Info(this, this.Coverage, Comments).JoinLines(" "));
            this.Line(this.Generator.GetBadges_Coverage(this, this.Coverage, Comments).JoinLines(" "));

            List<KeyValuePair<Type, MarkdownDocument_Type>> Types = this.Generator.GetAssemblyTypeMarkdown(this.Assembly);

            Dictionary<string, List<KeyValuePair<Type, MarkdownDocument_Type>>> NamespaceTypes = Types.Group(Type => Type.Key.Namespace);

            List<string> Namespaces = NamespaceTypes.Keys.List();

            Namespaces.Sort();

            this.Generator.GetBadges_Info(this, this.Coverage, Comments);
            this.Generator.GetBadges_Coverage(this, this.Coverage, Comments);

            Namespaces.Each(Namespace =>
                {
                this.HeaderUnderline(Namespace, Size: 2);

                List<KeyValuePair<Type, MarkdownDocument_Type>> NamespaceTypeMarkdown = NamespaceTypes[Namespace];

                NamespaceTypeMarkdown.Sort(Type => Type.Key.Name);

                // TODO namespace badges

                NamespaceTypeMarkdown.Each(Type =>
                    {
                    this.Header(this.Link(this.GetRelativePath(Type.Value.FilePath), Type.Key.GetGenericName()), Size: 4);

                    var TypeComments = Type.Value.TypeMeta.Comments;

                    this.Generator.GetBadges_Info(Type.Value, new TypeCoverage(Type.Key), TypeComments);
                    this.Generator.GetBadges_Coverage(Type.Value, new TypeCoverage(Type.Key), TypeComments);
                    });
                });

            this.Generator.WriteFooter(this);
            }
        }
    }