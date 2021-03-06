using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LDoc.Markdown.Manifest;
using System.Windows.Forms.DataVisualization.Charting;


// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedVariable
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable InconsistentNaming

// ReSharper disable VirtualMemberNeverOverriden.Global

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Implement this class to generate code for your assemblies and projects
    /// </summary>
    public abstract class SolutionMarkdownGenerator
        {
        #region Statics

        /// <summary>
        /// Default string to tag for language, (C#)
        /// </summary>
        public const string CSharpLanguage = "cs";

        /// <summary>
        /// Readme file name, default is "README.md"
        /// </summary>
        public const string MarkdownPath_RootFile = "README.md";

        /// <summary>
        /// Returns a link to a non-generic system type to get a link to its documentation.
        /// </summary>
        public static string MicrosoftSystemReferencePath(Type SystemType) =>
            "https://msdn.microsoft.com/en-us/library/" + $"{SystemType.FullyQualifiedName().ToLower().Before("[]")}.aspx";

        #endregion

        #region Reference Links

        /// <summary>
        /// Standard types linked from within LDoc, for convenience.
        /// </summary>
        protected static Dictionary<Type, string> ReferenceLinks => new Dictionary<Type, string>
            {
            [typeof(IDictionary<,>)] = "https://msdn.microsoft.com/en-us/library/s4ys34ea.aspx",
            [typeof(ICollection<>)] = "https://msdn.microsoft.com/en-us/library/92t2ye13.aspx",
            [typeof(IEquatable<>)] = "https://msdn.microsoft.com/en-us/library/ms131187.aspx",
            [typeof(IComparable<>)] = "https://msdn.microsoft.com/en-us/library/4d7sx9hd.aspx",
            [typeof(Expression<>)] = "https://msdn.microsoft.com/en-us/library/bb335710.aspx",
            [typeof(Nullable<>)] = "https://msdn.microsoft.com/en-us/library/b3h38hb0.aspx",
            [typeof(IEnumerable<>)] = "https://msdn.microsoft.com/en-us/library/78dfe2yb.aspx",
            [typeof(List<>)] = "https://msdn.microsoft.com/en-us/library/6sh2ey19.aspx",
            [typeof(Dictionary<,>)] = "https://msdn.microsoft.com/en-us/library/xfhwa508.aspx",
            [typeof(KeyValuePair<,>)] = "https://msdn.microsoft.com/en-us/library/5tbh8a42.aspx",
            [typeof(Action<>)] = "https://msdn.microsoft.com/en-us/library/018hxwa8.aspx",
            [typeof(Action<,>)] = "https://msdn.microsoft.com/en-us/library/bb549311.aspx",
            [typeof(Action<,,>)] = "https://msdn.microsoft.com/en-us/library/bb549392.aspx",
            [typeof(Action<,,,>)] = "https://msdn.microsoft.com/en-us/library/bb548654.aspx",
            [typeof(Action<,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd289012.aspx",
            [typeof(Action<,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd269635.aspx",
            [typeof(Action<,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd268304.aspx",
            [typeof(Action<,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd235351.aspx",
            [typeof(Action<,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd386922.aspx",
            [typeof(Action<,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd387291.aspx",
            [typeof(Action<,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402870.aspx",
            [typeof(Action<,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402748.aspx",
            [typeof(Action<,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402871.aspx",
            [typeof(Action<,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402866.aspx",
            [typeof(Action<,,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402873.aspx",
            [typeof(Action<,,,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402872.aspx",
            [typeof(Func<>)] = "https://msdn.microsoft.com/en-us/library/bb534960.aspx",
            [typeof(Func<,>)] = "https://msdn.microsoft.com/en-us/library/bb549151.aspx",
            [typeof(Func<,,>)] = "https://msdn.microsoft.com/en-us/library/bb534647.aspx",
            [typeof(Func<,,,>)] = "https://msdn.microsoft.com/en-us/library/bb549430.aspx",
            [typeof(Func<,,,,>)] = "https://msdn.microsoft.com/en-us/library/bb534303.aspx",
            [typeof(Func<,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd268303.aspx",
            [typeof(Func<,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd269654.aspx",
            [typeof(Func<,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd289456.aspx",
            [typeof(Func<,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd267613.aspx",
            [typeof(Func<,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd386894.aspx",
            [typeof(Func<,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd383294.aspx",
            [typeof(Func<,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402864.aspx",
            [typeof(Func<,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402863.aspx",
            [typeof(Func<,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402867.aspx",
            [typeof(Func<,,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402861.aspx",
            [typeof(Func<,,,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402868.aspx",
            [typeof(Func<,,,,,,,,,,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd402862.aspx",
            [typeof(Tuple<>)] = "https://msdn.microsoft.com/en-us/library/dd386941.aspx",
            [typeof(Tuple<,>)] = "https://msdn.microsoft.com/en-us/library/dd268536.aspx",
            [typeof(Tuple<,,>)] = "https://msdn.microsoft.com/en-us/library/dd387150.aspx",
            [typeof(Tuple<,,,>)] = "https://msdn.microsoft.com/en-us/library/dd414846.aspx",
            [typeof(Tuple<,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd414892.aspx",
            [typeof(Tuple<,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd386877.aspx",
            [typeof(Tuple<,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd387185.aspx",
            [typeof(Tuple<,,,,,,,>)] = "https://msdn.microsoft.com/en-us/library/dd383325.aspx"
            };

        #endregion

        #region Abstract 

        /// <summary>
        /// Override this member to specify the assemblies to generate documentation.
        /// </summary>
        public abstract Assembly[] DocumentAssemblies { get; }

        /// <summary>
        /// Write the markdown intro to your project, in the front page README.
        /// </summary>
        public abstract void Home_Intro(GeneratedDocument MD);

        /// <summary>
        /// Implement this member to specify the Root source control Url for the solution.
        /// </summary>
        public abstract string RootUrl { get; }


        #endregion

        #region Errors
        /// <summary>
        /// Stores types where documentation wasn't successfully located
        /// </summary>
        protected List<string> ErrorsReported { get; } = new List<string>();

        /// <summary>
        /// Adds an error to be reported within the markdown
        /// </summary>
        public void AddError(string Error)
            {
            if (!this.ErrorsReported.Has(Error))
                this.ErrorsReported.Add(Error);
            }

        /// <summary>
        /// Returns all errors reported during generation
        /// </summary>
        public List<string> GetErrors()
            {
            return this.ErrorsReported.List();
            }

        #endregion

        #region Documents

        /// <summary>
        /// Root documentation generated document
        /// </summary>
        public MarkdownDocument_Root Markdown_Root { get; set; }
        /// <summary>
        /// Table of contents generated document
        /// </summary>
        public MarkdownDocument_TableOfContents Markdown_TableOfContents { get; set; }

        /// <summary>
        /// Other titled markdown,
        /// Root readme, table of contents, coverage summary, 
        /// custom documents, etc.
        /// </summary>
        public Dictionary<string, GeneratedDocument> Markdown_Other { get; } = new Dictionary<string, GeneratedDocument>();

        /// <summary>
        /// Assembly-generated markdown documents.
        /// </summary>
        public Dictionary<Assembly, MarkdownDocument_Assembly> Markdown_Assembly { get; } =
            new Dictionary<Assembly, MarkdownDocument_Assembly>();

        /// <summary>
        /// Type-generated markdown documents.
        /// </summary>
        public Dictionary<Type, MarkdownDocument_Type> Markdown_Type { get; } = new Dictionary<Type, MarkdownDocument_Type>();

        /// <summary>
        /// Member-generated markdown documents.
        /// </summary>
        public Dictionary<MemberInfo, MarkdownDocument_Member> Markdown_Member { get; } =
            new Dictionary<MemberInfo, MarkdownDocument_Member>();

        /// <summary>
        /// Method group-generated markdown documents.
        /// </summary>
        public Dictionary<MethodInfo[], MarkdownDocument_MethodGroup> Markdown_MethodGroups { get; } =
            new Dictionary<MethodInfo[], MarkdownDocument_MethodGroup>();



        #endregion

        #region Statistics

        /// <summary>
        /// Keeps track of generation statistics
        /// </summary>
        public GeneratorStatistics Stats { get; } = new GeneratorStatistics();

        #endregion

        #region Colors

        /// <summary>
        /// Override this value with an array of EXACTLY 4 numbers to specify the levels of coloring
        /// Default is: `{ 30, 50, 70, 100 }`
        /// 
        /// Percentage  | Color
        /// ---         | ---
        /// 30-         | BadgeColor.Red
        /// 31-50       | BadgeColor.Yellow
        /// 51-70       | BadgeColor.YellowGreen
        /// 71-100      | BadgeColor.Green
        /// 100         | BadgeColor.BrightGreen
        /// 101+        | BadgeColor.Blue
        /// </summary>
        public virtual int[] ColorThresholds => new[] { 30, 50, 70, 100 };

        /// <summary>
        /// Gets a BadgeColor for a given <paramref name="Percentage"/>
        /// 
        /// Override this method to customize the deciding of BadgeColor by Percentage.
        /// </summary>
        public virtual BadgeColor GetColorByPercentage(int Percentage)
            {
            if (Percentage < this.ColorThresholds[0])
                return BadgeColor.Red;
            if (Percentage < this.ColorThresholds[1])
                return BadgeColor.Yellow;
            if (Percentage < this.ColorThresholds[2])
                return BadgeColor.YellowGreen;
            if (Percentage < this.ColorThresholds[3])
                return BadgeColor.Green;
            if (Percentage == this.ColorThresholds[3])
                return BadgeColor.BrightGreen;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (Percentage > this.ColorThresholds[3])
                return BadgeColor.Blue;

            return BadgeColor.LightGrey;
            }


        /// <summary>
        /// Override this member to customize colors used for badges
        /// </summary>
        public virtual ColorSettings Colors { get; } = new ColorSettings();


        #endregion

        #region Language

        /// <summary>
        /// Override this value or set its properties to customize the text used for markdown generation.
        /// </summary>
        public virtual Text Language => new Text();

        #endregion

        #region Document Generators

        /// <summary>
        /// Generates root markdown document (front page)
        /// </summary>
        public virtual MarkdownDocument_Root GenerateRootMarkdown()
            {
            this.Markdown_Root = new MarkdownDocument_Root(this, this.Language.MainReadme);
            return this.Markdown_Root;
            }

        /// <summary>
        /// Generates table of contents document
        /// </summary>
        public virtual MarkdownDocument_TableOfContents GenerateTableOfContentsMarkdown()
            {
            this.Markdown_TableOfContents = new MarkdownDocument_TableOfContents(this, this.Language.TableOfContents);
            return this.Markdown_TableOfContents;
            }

        /// <summary>
        /// Generates coverage summary document
        /// </summary>
        public virtual MarkdownDocument_TableOfContents GenerateCoverageSummaryMarkdown()
            {
            return new MarkdownDocument_TableOfContents(this, this.Language.CoverageSummary);
            }

        /// <summary>
        /// Generates markdown for an Assembly
        /// </summary>
        public virtual MarkdownDocument_Assembly GenerateMarkdown(Assembly Assembly)
            {
            return new MarkdownDocument_Assembly(Assembly, this, Assembly.GetName().Name);
            }

        /// <summary>
        /// Generates markdown for a Type
        /// </summary>
        public virtual MarkdownDocument_Type GenerateMarkdown(Type Type)
            {
            return new MarkdownDocument_Type(Type, this, Type.Name);
            }

        /// <summary>
        /// Generates markdown for a group of methods
        /// </summary>
        public virtual MarkdownDocument_MethodGroup GenerateMarkdown(MethodInfo[] MethodGroup)
            {
            return new MarkdownDocument_MethodGroup(MethodGroup, this, $"{MethodGroup.First()?.Name} + {MethodGroup.Length - 1} Overloads");
            }

        /// <summary>
        /// Generates markdown for a Member
        /// </summary>
        public virtual MarkdownDocument_Member GenerateMarkdown(MemberInfo Member)
            {
            return new MarkdownDocument_Member(Member, this, Member.Name);
            }

        /// <summary>
        /// Generates markdown for a tag summary 
        /// </summary>
        public virtual MarkdownDocument_TagSummary GenerateTagSummaryMarkdown(string Tag)
            {
            return new MarkdownDocument_TagSummary(this, $"Summary '{Tag}'", Tag);
            }


        #endregion

        #region Display + 

        #region Images

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        public virtual string BannerImage_Large([NotNull] GeneratedDocument MD) => "";

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        public virtual string BannerImage_Small([NotNull] GeneratedDocument MD) => "";

        /// <summary>
        /// Override this value to display a large image in the upper right corner of the main document
        /// </summary>
        public virtual string LogoImage_Large([NotNull] GeneratedDocument MD) => "";

        /// <summary>
        /// Override this value to display a small image in the upper right corner of sub-documents
        /// </summary>
        public virtual string LogoImage_Small([NotNull] GeneratedDocument MD) => "";


        #endregion

        #region Root

        /// <summary>
        /// Override this value to link to related projects at the bottom of your home document
        /// </summary>
        public virtual List<ProjectInfo> Home_RelatedProjects => new List<ProjectInfo>();

        /// <summary>
        /// Override this value to link to project dependencies at the bottom of your home document
        /// </summary>
        public virtual List<ProjectInfo> Home_DependencyProjects => new List<ProjectInfo>();

        /// <summary>
        /// Override this to provide custom badge urls for the project.
        /// </summary>
        public virtual string[] CustomBadgeUrls => new string[] { };


        /// <summary>
        /// Override this value to indicate installation instructions.
        /// </summary>
        public virtual void HowToInstall([NotNull] GeneratedDocument MD)
            {
            }

        #endregion

        #region Assembly

        /// <summary>
        /// Override or add values to this dictionary to specify <see cref="Assembly"/> comments.
        /// </summary>
        public virtual Dictionary<Assembly, ICodeComment> AssemblyComments { get; }
            = new Dictionary<Assembly, ICodeComment>();

        #endregion

        #region Header

        /// <summary>
        /// Writes the default header, including the large or small
        /// banner and logo
        /// </summary>
        public virtual void WriteHeader(GeneratedDocument MD)
            {
            bool IsMain = MD.FullPath == this.Markdown_Root.FullPath;


            if (IsMain && !string.IsNullOrEmpty(this.BannerImage_Large(MD)))
                {
                MD.Line(MD.Image(this.BannerImage_Large(MD)));
                }
            else if (!string.IsNullOrEmpty(this.BannerImage_Small(MD)))
                {
                MD.Line(MD.Image(this.BannerImage_Small(MD)));
                }

            if (IsMain && !string.IsNullOrEmpty(this.LogoImage_Large(MD)))
                {
                MD.Line(MD.Image(this.LogoImage_Large(MD), this.Language.AltText_Logo, L.Align.Right));
                }
            else if (!string.IsNullOrEmpty(this.LogoImage_Small(MD)))
                {
                MD.Line(MD.Link(MD.GetRelativePath(this.Markdown_Root.FullPath), MD.Image(this.LogoImage_Small(MD), this.Language.AltText_Logo, L.Align.Right)));
                }
            }


        #endregion

        #region Footer



        /// <summary>
        /// Writes the footer to a markdown document.
        /// </summary>
        public void WriteFooter([NotNull] GeneratedDocument MD)
            {
            MD.Line("");
            MD.Line("");
            MD.HorizontalRule();

            this.WriteCustomFooter(MD);
            MD.Line("");
            MD.Line(new[]
                {
                $"This markdown was generated by {MD.Link(LDoc.Urls.GitHubUrl, nameof(LDoc))}, " +
                $"powered by {MD.Link(LUnit.LUnit.Urls.GitHubRepository_LUnit, nameof(LUnit))}, " +
                $"{MD.Link(LUnit.LUnit.Urls.GitHubRepository_LCore, nameof(LCore))}"
                }.JoinLines(" "));
            }

        /// <summary>
        /// Writes the footer for your markdown document
        /// </summary>
        public virtual void WriteCustomFooter(GeneratedDocument MD)
            {
            MD.Line(new[]
                {
                $"Copyright {DateTime.Now.Year} &copy;",
                MD.Link(MD.GetRelativePath(this.Markdown_Root.FullPath)),
                MD.Link(MD.GetRelativePath(this.Markdown_TableOfContents.FullPath))
                }.JoinLines(" "));
            }



        #endregion

        #endregion

        #region Loading

        private void Load(Assembly Assembly)
            {
            Assembly.GetExportedTypes().Select(this.IncludeType).Each(this.Load);

            this.Markdown_Assembly.Add(Assembly, this.GenerateMarkdown(Assembly));
            }

        private void Load(Type Type)
            {
            MemberInfo[] AllMembers = Type.GetMembers().Select(this.IncludeMember);

            Dictionary<string, List<MethodInfo>> Methods = AllMembers
                .Filter<MethodInfo>()
                .Group(Method => Method.Name);

            List<KeyValuePair<string, List<MethodInfo>>> MethodGroups = Methods.Select(Method => Method.Value.Count > 1);

            MethodGroups.Convert(Group => Group.Value.Array()).Each(this.Load);
            AllMembers.Each(this.Load);

            this.Markdown_Type.Add(Type, this.GenerateMarkdown(Type));
            }

        private void Load(MemberInfo Member)
            {
            this.Markdown_Member.Add(Member, this.GenerateMarkdown(Member));
            }

        private void Load(MethodInfo[] MethodGroup)
            {
            this.Markdown_MethodGroups.Add(MethodGroup, this.GenerateMarkdown(MethodGroup));
            }


        #endregion

        #region File Paths

        /// <summary>
        /// Root path of the current running solution (development ONLY)
        /// </summary>
        public virtual string GeneratedMarkdownRoot => L.Ref.GetSolutionRootPath();

        /// <summary>
        /// Returns the root directory for an assembly, which is a folder beneath the <param name="Assembly"></param> 
        /// root directory
        /// </summary>
        public virtual string MarkdownPath_AssemblyRoot(Assembly Assembly) =>
            $"{Assembly.GetRootPath()}\\{this.MarkdownPath_Documentation}";

        /// <summary>
        /// Returns the root directory for the repository, which is the root directory for markdown generation
        /// </summary>
        public virtual string MarkdownPath_MemberRoot(MemberInfo Member) =>
            this.MarkdownPath_AssemblyRoot(Member.GetAssembly());

        /// <summary>
        /// Documents folder, default is "docs"
        /// </summary>
        protected virtual string MarkdownPath_Documentation => "docs";


        /// <summary>
        /// Generates the markdown path for the data manifest JSON file
        /// </summary>
        public virtual string MarkdownPath_DataManifest =>
            $"{L.Ref.GetSolutionRootPath()}\\" +
            $"{this.Language.ManifestFile}";


        /// <summary>
        /// Generates the markdown path for the type manifest JSON file
        /// </summary>
        public string MarkdownPath_TypeManifest =>
            $"{L.Ref.GetSolutionRootPath()}\\" +
            $"{this.Language.TypeManifestFile}";
        #endregion

        #region Inclusion Conditions

        /// <summary>
        /// Determines if a Type should be included in documentation
        /// </summary>
        public virtual bool IncludeType([NotNull] Type Type) =>
            !Type.HasAttribute<IExcludeFromMarkdownAttribute>();

        /// <summary>
        /// Determines if a Member should be included in documentation
        /// </summary>
        public virtual bool IncludeMember([NotNull] MemberInfo Member) =>
            // Manually excluded members and declaring types
            !Member.HasAttribute<IExcludeFromMarkdownAttribute>() &&
            Member.DeclaringType?.HasAttribute<IExcludeFromMarkdownAttribute>() != true &&
            // Property getters and setters are handled under the PropertyInfo member, not the get and set methods
            !(Member is MethodInfo && ((MethodInfo)Member).IsPropertyGetterOrSetter()) &&
            // Don't include members that are being exposed from base classes
            Member.IsDeclaredMember() &&
            // Don't make a document for each enum value
            Member.DeclaringType?.IsEnum == false &&
            // TODO enable constructors
            !(Member is ConstructorInfo);

        #endregion

        #region Links

        /// <summary>
        /// Override this value to supply custom links to foreign types.
        /// </summary>
        public virtual Dictionary<Type, string> CustomTypeLinks => new Dictionary<Type, string>();

        /// <summary>
        /// Links to a string of a possibly unknown type or member
        /// </summary>
        public virtual string LinkToString(GeneratedDocument MD, string Link)
            {
            // TODO resolve link from type name

            // TODO resolve link from sibling member

            // TODO resolve link from member parameter

            return Link;
            }

        /// <summary>
        /// Gets a link to a type, whether it is public to this project, a type on GitHub,
        /// a type in related projects
        /// or a System type.
        /// 
        /// Otherwise, fall back on a google search.
        /// </summary>
        public virtual string LinkToType(GeneratedDocument MD, Type Type, bool AsHtml = false)
            {
            // Unassigned types, (T, etc), need no link
            if (Type.IsGenericParameter)
                return Type.Name;

            // Ref types, link to actual type
            if (Type.IsByRef)
                return $"ref {this.LinkToType(MD, Type.GetElementType(), AsHtml)}";

            // Array types, nest array symbols outside of the link
            if (Type.IsArray)
                return $"{this.LinkToType(MD, Type.GetElementType(), AsHtml)}[]";

            // Generic types, display in standard generic notation and link each type individually. Recursion is required.
            if (Type.IsGenericType && !Type.IsGenericTypeDefinition)
                {
                string GenericTypeLink = this.LinkToType(MD, Type.GetGenericTypeDefinition(), AsHtml);
                Type[] Parameters = Type.GenericTypeArguments;

                return $"{GenericTypeLink}&lt;{Parameters.Convert(Param => this.LinkToType(MD, Param, AsHtml)).Combine(", ")}&gt;";
                }


            // ---- From here on Type is known to not be generic, unknown generic parameter, ref, or array type ----

            string Name = Type.GetNestedNames().Before("<");

            // Local links for known documented types
            string TypeLink = this.Markdown_Type.First(MDType => MDType.Key == Type).Value?.FullPath;
            // bold local links
            if (!string.IsNullOrEmpty(TypeLink))
                return MD.Bold(MD.Link(MD.GetRelativePath(TypeLink), Name, AsHtml: AsHtml), AsHtml);


            // Resolve custom type links from implementation
            if (this.CustomTypeLinks.ContainsKey(Type))
                return MD.Link(this.CustomTypeLinks[Type], Name, "", TargetNewWindow: true, AsHtml: AsHtml);

            // Resolve default known type links
            if (ReferenceLinks.ContainsKey(Type))
                return MD.Link(ReferenceLinks[Type], Name, "", TargetNewWindow: true, AsHtml: AsHtml);

            // Resolve all non-generic System types
            if (!Type.ContainsGenericParameters && // Generic parameters aren't supported with named links on their docs
                Type.FullyQualifiedName().ToLower().StartsWith("system."))
                {
                this.Stats.SystemLinks++;
                return MD.Link(MicrosoftSystemReferencePath(Type), Name, AsHtml: AsHtml);
                }

            if (this.DocumentAssemblies.Has(Type.GetAssembly()))
                return MD.Link(MD.GetRelativePath(this.Markdown_Type[Type].FullPath), Name, AsHtml: AsHtml);

            // Search all known manifests for the type
            foreach (var Project in this.Home_RelatedProjects)
                foreach (var Manifest in Project.Manifests)
                    if (Manifest != null)
                        foreach (var Document in Manifest.MemberDocuments)
                            if (Document.MemberName == Type.FullyQualifiedName())
                                {
                                this.Stats.LDocLinks++;
                                return MD.Link(Document.FullUrl_Documentation, Name, AsHtml: AsHtml);
                                }

            // Search all dependency manifests for the type
            foreach (var Project in this.Home_DependencyProjects)
                foreach (var Manifest in Project.Manifests)
                    foreach (var Document in Manifest.MemberDocuments)
                        if (Document.MemberName == Type.FullyQualifiedName())
                            {
                            this.Stats.LDocLinks++;
                            return MD.Link(Document.FullUrl_Documentation, Name, AsHtml: AsHtml);
                            }

            // Execution past here is considered 'failure'


            if (!this.ErrorsReported.Has(Type))
                this.ErrorsReported.Add($"Could not find type link for {Type.FullyQualifiedName()}");

            this.Stats.ExternalLinks++;

            return MD.Link("https://www.google.com/#q=C%23+" +
                           $"{WebUtility.HtmlEncode(Type.FullyQualifiedName())}",
                Name,
                $"Search for '{WebUtility.HtmlEncode(Type.FullyQualifiedName())}'",
                TargetNewWindow: true, AsHtml: AsHtml);
            }


        #endregion

        #region Comments

        /// <summary>
        /// Formats comments, properly displaying comment tags.
        /// </summary>
        public virtual string FormatComment(GeneratedDocument MD, string CommentText)
            {
            CommentText.Matches("<code>(.*)</code>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    $"\r\n```\r\n{Match.Groups[groupnum: 1]}\r\n```\r\n"));

            CommentText.Matches("<value>(.*)</value>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    $"` {Match.Groups[groupnum: 1]} `"));

            CommentText.Matches("<see cref=\"(.*)\"/>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    this.LinkToString(MD, Match.Groups[groupnum: 1].Value)));

            CommentText.Matches("<seealso cref=\"(.*)\"/>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    this.LinkToString(MD, Match.Groups[groupnum: 1].Value)));

            CommentText.Matches("<paramref name=\"(.*)\"/>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    this.LinkToString(MD, Match.Groups[groupnum: 1].Value)));

            CommentText.Matches("<typeparamref name=\"(.*)\"/>").Each(Match =>
                CommentText = CommentText.Replace(Match.Groups[groupnum: 0].Value,
                    this.LinkToString(MD, Match.Groups[groupnum: 1].Value)));

            return CommentText;
            }

        /// <summary>
        /// Override this to specify custom comment tags to track. 
        /// By default only todo and bug are tracked (upper case only)
        /// for example, 
        /// 
        /// public override string[] CustomCommentTags => new string[] { "CustomTag" };
        /// 
        /// Will track all instances of:
        /// 
        ///  "// CustomTag ... "
        ///  "//CustomTag ... " 
        /// 
        /// </summary>
        public virtual string[] CustomCommentTags => new string[] { };

        /// <summary>
        /// Override this value to determine custom colors depending on the count
        /// of the particular custom tag found.
        /// </summary>
        public virtual Dictionary<string, Func<uint, BadgeColor>> CustomCommentColor => new Dictionary<string, Func<uint, BadgeColor>>();


        #endregion

        #region Testing

        /// <summary>
        /// Override this member to specify any test assemblies
        /// </summary>
        public virtual Assembly[] TestAssemblies => new Assembly[] { };

        /// <summary>
        /// Override this value to disable LUnit Unit test coverage tracking by Trait.
        /// Default is true.
        /// </summary>
        public virtual bool DocumentUnitCoverage => true;

        /// <summary>
        /// Override this value to disable LUnit Attribute test coverage tracking.
        /// Default is true.
        /// </summary>
        public virtual bool DocumentAttributeCoverage => true;
        #endregion

        #region Helpers



        /// <summary>
        /// Gets all markdown generated by the generator.
        /// </summary>
        public List<GeneratedDocument> GetAllMarkdown()
            {
            var AllMarkdown = new List<GeneratedDocument>();

            AllMarkdown.AddRange(this.Markdown_Assembly.Values);
            AllMarkdown.AddRange(this.Markdown_Type.Values);
            AllMarkdown.AddRange(this.Markdown_Member.Values);
            AllMarkdown.AddRange(this.Markdown_Other.Values);

            return AllMarkdown;
            }

        /// <summary>
        /// Locates a markdown document for a particular <paramref name="Member"/>
        /// </summary>
        public MarkdownDocument_Member FindMarkdown(MemberInfo Member)
            {
            return this.Markdown_Member.First(MD => MD.Key == Member).Value;
            }

        /// <summary>
        /// Override this method to generate custom documents for your project.
        /// </summary>
        public virtual Dictionary<string, GeneratedDocument> GetOtherDocuments()
            {
            return new Dictionary<string, GeneratedDocument>();
            }


        /// <summary>
        /// Get all Member group markdown owned by a given <paramref name="Type"/>
        /// </summary>
        public List<KeyValuePair<MemberInfo, MarkdownDocument_Member>> GetTypeMemberMarkdown(Type Type)
            {
            return this.Markdown_Member.Select(Member => Member.Key.DeclaringType == Type);
            }

        /// <summary>
        /// Get all Type markdown for a given <paramref name="Assembly"/>
        /// </summary>
        public virtual List<KeyValuePair<Type, MarkdownDocument_Type>> GetAssemblyTypeMarkdown(Assembly Assembly)
            {
            return this.Markdown_Type.Select(Type => Type.Key.GetAssembly()?.GetName().Name == Assembly.GetName().Name);
            }


        /// <summary>
        /// Returns the index of a declared member. 
        /// This corresponds to the order the member is defined in the file.
        /// </summary>
        public string GetMethodIndex(MemberInfo Member)
            {
            if (Member is MethodInfo && Member.DeclaringType?.GetMember(Member.Name).Length > 1)
                return $"-{Member.DeclaringType?.GetMember(Member.Name).IndexOf(o => o == Member)}";

            return "";
            }



        #endregion

        #region Charting

        /// <summary>
        /// Enables generation and use of of charts 
        /// </summary>
        protected virtual bool EnableChartGeneration => true;

        /// <summary>
        /// Save an image chart of a collection split into categories
        /// </summary>
        public void SaveCategoryPieChart<T>(IEnumerable<T> Collection, Func<T, string> Categorizer, string FullPath, Size? Size = null)
            {
            Dictionary<string, List<T>> GroupedCollection = Collection.Group(Categorizer);

            Size = Size ?? new Size(width: 350, height: 250);

            using (var Chart = new Chart())
                {
                Chart.Size = (Size)Size;

                Chart.BackColor = Color.Transparent;

                Chart.ChartAreas.Add(new ChartArea());
                Chart.Palette = ChartColorPalette.None;

                uint TotalCount = GroupedCollection.Sum(Item => Item.Value.Count);

                var Series = Chart.Series.Add("series");

                Series.IsVisibleInLegend = true;

                Series.CustomProperties = "PieLabelStyle=Outside";

                GroupedCollection.Each((i, Item) =>
                    {

                        Series.ChartType = SeriesChartType.Doughnut;

                        Series.Points.Add(Item.Value.Count);
                        Series.SmartLabelStyle = new AnnotationSmartLabelStyle
                            {
                            AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes
                            };

                        var Point = Series.Points[i];

                        Point.XValue = Item.Value.Count;

                        //  Point.Color = Colors.GetAt(i);
                        Point.BackGradientStyle = GradientStyle.VerticalCenter;
                        Point.BackSecondaryColor = Color.FromArgb(alpha: 150, baseColor: Point.Color);

                        Point.IsVisibleInLegend = true;
                        Point.LabelBackColor = Color.FromArgb(alpha: 160, baseColor: Color.Gray);

                        Point.Font = new Font(Point.Font.FontFamily, emSize: 12f);

                        Point.AxisLabel = $"{Item.Key.Pluralize()}".ReplaceAll(" ", "\r\n") +
                                          $"\r\n{Item.Value.Count} ({Item.Value.Count.PercentageOf((int)TotalCount)}%)";
                        Point.LegendText = $"{Item.Key}";
                    });

                Chart.ChartAreas[index: 0].Area3DStyle.Enable3D = true;
                Chart.ChartAreas[index: 0].Area3DStyle.Inclination = 15;
                Chart.ChartAreas[index: 0].Area3DStyle.IsClustered = true;

                Chart.SaveImage(FullPath, ChartImageFormat.Png);
                }
            }

        #endregion

        #region Manifests

        /// <summary>
        /// Control the generation of data historical manifest.
        /// This will track all changes to code over time.
        /// </summary>
        public virtual bool EnableDataManifest => false;

        /// <summary>
        /// Control the generation of type manifest.
        /// This enables cross-project type linking.
        /// </summary>
        public virtual bool EnableTypeManifest => true;

        private void GenerateManifests(List<GeneratedDocument> AllMarkdown)
            {
            this.GenerateTypeManifest(AllMarkdown);
            this.GenerateDataManifest(AllMarkdown);
            }

        /// <summary>
        /// Data manifest
        /// </summary>
        public LDocDataManifest Manifest_Data { get; protected set; }
        /// <summary>
        /// Type manifest
        /// </summary>
        public LDocTypeManifest Manifest_Type { get; protected set; }

        private void GenerateDataManifest(List<GeneratedDocument> AllMarkdown)
            {
            if (this.Manifest_Data == null)
                this.Manifest_Data = new LDocDataManifest(AllMarkdown);
            else
                this.Manifest_Data.CaptureHistory(AllMarkdown);
            string JSON = this.Manifest_Data.CreateManifestJSON();
            File.WriteAllText(this.MarkdownPath_DataManifest, JSON);
            }


        private void GenerateTypeManifest(List<GeneratedDocument> AllMarkdown)
            {
            if (this.Manifest_Type == null)
                this.Manifest_Type = new LDocTypeManifest(AllMarkdown);
            else
                this.Manifest_Type.CaptureHistory(AllMarkdown);

            string JSON = this.Manifest_Type.CreateManifestJSON();
            File.WriteAllText(this.MarkdownPath_TypeManifest, JSON);
            }

        private void LoadManifests()
            {
            this.LoadDataManifest();
            this.LoadTypeManifest();
            }

        private void LoadTypeManifest()
            {
            try
                {
                if (this.EnableTypeManifest)
                    {
                    string JSON = File.ReadAllText(this.MarkdownPath_TypeManifest);
                    this.Manifest_Type = LDocTypeManifest.FromJSON(JSON);
                    }
                }
            catch
                {
                }
            }

        private void LoadDataManifest()
            {
            try
                {
                if (this.EnableDataManifest)
                    {
                    string JSON = File.ReadAllText(this.MarkdownPath_DataManifest);
                    this.Manifest_Data = LDocDataManifest.FromJSON(JSON);
                    }
                }
            catch
                {
                }
            }

        #endregion


        /// <summary>
        /// Generates all markdown documentation, optionally writing all files to disk using <paramref name="WriteToDisk"/>. 
        /// </summary>
        public void Generate(bool WriteToDisk = false)
            {
            // Generates all assembly, type, and member Markdown
            this.DocumentAssemblies.Each(this.Load);

            // Generate root markdown
            this.Markdown_Other.Add(this.Language.MainReadme, this.GenerateRootMarkdown());

            // Generate coverage summary markdown
            this.Markdown_Other.Add(this.Language.CoverageSummary, this.GenerateCoverageSummaryMarkdown());

            // Generate any custom markdown
            this.GetOtherDocuments().Each(Document => this.Markdown_Other.Add(Document.Key, Document.Value));


            // Add todo summary
            this.Markdown_Other.Add("TODO Summary", this.GenerateTagSummaryMarkdown("TODO"));
            // Add bug summary
            this.Markdown_Other.Add("BUG Summary", this.GenerateTagSummaryMarkdown("BUG"));
            // Add not implemented summary
            this.Markdown_Other.Add("Not Implemented Summary", this.GenerateTagSummaryMarkdown("throw new NotImplementedException"));
            // Add not implemented summary
            this.CustomCommentTags.Each(
                Tag => this.Markdown_Other.Add($"{Tag} Summary", this.GenerateTagSummaryMarkdown(Tag)));


            // Lastly, generate table of contents
            this.Markdown_Other.Add(this.Language.TableOfContents, this.GenerateTableOfContentsMarkdown());

            // Load existing manifests
            this.LoadManifests();

            List<GeneratedDocument> AllMarkdown = this.GetAllMarkdown();

            if (WriteToDisk)
                {
                AllMarkdown.Each(MD =>
                {
                    MD.Generate();

                    string Path = MD.FullPath;

                    // just to be safe
                    if (Path.EndsWith(".md"))
                        {
                        Path.EnsurePathExists();

                        File.WriteAllLines(Path, MD.GetMarkdownLines().Array());

                        MD.Clear();
                        }
                    else
                        {
                        this.ErrorsReported.Add($"File generated was not a markdown file: {Path}");
                        }
                });

                this.GenerateManifests(AllMarkdown);
                }
            }
        }
    }