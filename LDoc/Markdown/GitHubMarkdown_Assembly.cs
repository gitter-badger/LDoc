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
    public class GitHubMarkdown_Assembly : GitHubMarkdown
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
        public GitHubMarkdown_Assembly(Assembly Assembly, MarkdownGenerator Generator, string FilePath, string Title) : base(Generator, FilePath, Title)
            {
            this.Assembly = Assembly;
            this.Coverage = new AssemblyCoverage(Assembly);

            this.Generate();
            }

        private void Generate()
            {
            var MarkdownGenerator = this.Generator;
            if (MarkdownGenerator != null)
                {
                MarkdownGenerator.WriteHeader(this);

                ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                this.Line(this.Link(this.GetRelativePath(MarkdownGenerator.MarkdownPath_Root), MarkdownGenerator.Language.LinkText_Home));

                this.Header($"{this.Assembly.GetName().Name}", Size: 2);

                this.Line(MarkdownGenerator.GetBadges_Info(this, this.Coverage, Comments).JoinLines(" "));
                this.Line(MarkdownGenerator.GetBadges_Coverage(this, this.Coverage, Comments).JoinLines(" "));

                List<KeyValuePair<Type, GitHubMarkdown_Type>> Types = MarkdownGenerator.GetAssemblyTypeMarkdown(this.Assembly);

                Dictionary<string, List<KeyValuePair<Type, GitHubMarkdown_Type>>> NamespaceTypes = Types.Group(Type => Type.Key.Namespace);

                List<string> Namespaces = NamespaceTypes.Keys.List();

                Namespaces.Sort();

                MarkdownGenerator.GetBadges_Info(this, this.Coverage, Comments);
                MarkdownGenerator.GetBadges_Coverage(this, this.Coverage, Comments);

                Namespaces.Each(Namespace =>
                {
                    this.HeaderUnderline(Namespace, Size: 2);

                    List<KeyValuePair<Type, GitHubMarkdown_Type>> NamespaceTypeMarkdown = NamespaceTypes[Namespace];

                    NamespaceTypeMarkdown.Sort(Type => Type.Key.Name);

                    // TODO namespace badges

                    NamespaceTypeMarkdown.Each(Type =>
                    {
                        this.Header(this.Link(this.GetRelativePath(Type.Value.FilePath), Type.Key.GetGenericName()));

                        var TypeComments = Type.Value.TypeMeta.Comments;

                        MarkdownGenerator.GetBadges_Info(Type.Value, new TypeCoverage(Type.Key), TypeComments);
                        MarkdownGenerator.GetBadges_Coverage(Type.Value, new TypeCoverage(Type.Key), TypeComments);

                    });
                });

                MarkdownGenerator.WriteFooter(this);
                }
            }
        }
    }