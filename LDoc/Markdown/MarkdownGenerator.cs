using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;
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
    public abstract class MarkdownGenerator
        {
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

        public GitHubMarkdown_MemberGroup FindMarkdown(MemberInfo Member)
            {
            return this.Markdown_Member.First(MD => MD.Key.Has(Member)).Value;
            }

        #region ReferenceLinks
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

            /*

                        [typeof(object)] = "https://msdn.microsoft.com/en-us/library/system.object.aspx",

                        [typeof(void)] = "https://msdn.microsoft.com/en-us/library/system.void.aspx",

                        [typeof(bool)] = "https://msdn.microsoft.com/en-us/library/system.boolean.aspx",

                        [typeof(string)] = "https://msdn.microsoft.com/en-us/library/system.string.aspx",

                        [typeof(int)] = "https://msdn.microsoft.com/en-us/library/system.int32.aspx",
                        [typeof(byte)] = "https://msdn.microsoft.com/en-us/library/system.byte.aspx",
                        [typeof(sbyte)] = "https://msdn.microsoft.com/en-us/library/system.sbyte.aspx",
                        [typeof(decimal)] = "https://msdn.microsoft.com/en-us/library/system.decimal.aspx",
                        [typeof(ulong)] = "https://msdn.microsoft.com/en-us/library/system.uint64.aspx",
                        [typeof(ushort)] = "https://msdn.microsoft.com/en-us/library/system.uint16.aspx",
                        [typeof(long)] = "https://msdn.microsoft.com/en-us/library/system.int64.aspx",
                        [typeof(short)] = "https://msdn.microsoft.com/en-us/library/system.int16.aspx",
                        [typeof(float)] = "https://msdn.microsoft.com/en-us/library/system.single.aspx",
                        [typeof(double)] = "https://msdn.microsoft.com/en-us/library/system.double.aspx",
                        [typeof(char)] = "https://msdn.microsoft.com/en-us/library/system.char.aspx",

                        [typeof(IComparable)] = "https://msdn.microsoft.com/en-us/library/system.icomparable.aspx",
                        [typeof(IEnumerable)] = "https://msdn.microsoft.com/en-us/library/system.collections.ienumerable.aspx",
                        [typeof(IConvertible)] = "https://msdn.microsoft.com/en-us/library/system.iconvertible.aspx",

                        [typeof(Assembly)] = "https://msdn.microsoft.com/en-us/library/system.reflection.assembly.aspx",
                        [typeof(Type)] = "https://msdn.microsoft.com/en-us/library/system.type.aspx",
                        [typeof(MemberInfo)] = "https://msdn.microsoft.com/en-us/library/system.reflection.memberinfo.aspx",
                        [typeof(MethodInfo)] = "https://msdn.microsoft.com/en-us/library/system.reflection.methodinfo.aspx",
                        [typeof(DateTime)] = "https://msdn.microsoft.com/en-us/library/system.datetime.aspx",
                        [typeof(TimeSpan)] = "https://msdn.microsoft.com/en-us/library/system.timespan.aspx",
                        [typeof(Exception)] = "https://msdn.microsoft.com/en-us/library/system.exception.aspx",
                        [typeof(Tuple)] = "https://msdn.microsoft.com/en-us/library/system.tuple.aspx",
                        [typeof(Nullable)] = "https://msdn.microsoft.com/en-us/library/system.nullable.aspx",
           
                        [typeof(Action)] = "https://msdn.microsoft.com/en-us/library/system.action.aspx",
            */
            };

        #endregion

        /// <summary>
        /// Override this member to specify the assemblies to generae documentation.
        /// </summary>
        public abstract Assembly[] DocumentAssemblies { get; }

        /// <summary>
        /// Write the markdown intro to your project, in the front page README.
        /// </summary>
        public abstract void Home_Intro(GitHubMarkdown MD);

        /// <summary>
        /// Override this value to link to related projects at the bottom of your home document
        /// </summary>
        public virtual List<ProjectInfo> Home_RelatedProjects => new List<ProjectInfo>();

        /// <summary>
        /// Override this value to indicate installation instructions.
        /// </summary>
        public virtual void HowToInstall([NotNull] GitHubMarkdown MD)
            {

            }

        //https://codesingularity.visualstudio.com/_apis/public/build/definitions/ef4060d7-9700-4b9c-acc3-e2263d774197/3/badge
        // TODO hook custom badge urls
        /// <summary>
        /// Override this to provide custom badge urls for the project.
        /// </summary>
        public virtual string[] CustomBadgeUrls => new string[] { };

        /// <summary>
        /// Override this value to supply custom links to foreign types.
        /// <see cref="RequireDirectLinksToAllForeignTypes"/>
        /// </summary>
        public virtual Dictionary<Type, string> CustomTypeLinks => new Dictionary<Type, string>();

        #region Variables + 

        /// <summary>
        /// Other titled markdown,
        /// Root readme, table of contents, coverage summary, 
        /// custom documents, etc.
        /// </summary>
        public Dictionary<string, GitHubMarkdown> Markdown_Other { get; } = new Dictionary<string, GitHubMarkdown>();

        /// <summary>
        /// Assembly-generated markdown documents.
        /// </summary>
        public Dictionary<Assembly, GitHubMarkdown_Assembly> Markdown_Assembly { get; } =
            new Dictionary<Assembly, GitHubMarkdown_Assembly>();

        /// <summary>
        /// Type-generated markdown documents.
        /// </summary>
        public Dictionary<Type, GitHubMarkdown_Type> Markdown_Type { get; } = new Dictionary<Type, GitHubMarkdown_Type>();

        /// <summary>
        /// Member-generated markdown documents.
        /// </summary>
        public Dictionary<MemberInfo[], GitHubMarkdown_MemberGroup> Markdown_Member { get; } =
            new Dictionary<MemberInfo[], GitHubMarkdown_MemberGroup>();


        #endregion

        #region Generators + 



        /// <summary>
        /// Generates root markdown document (front page)
        /// </summary>
        public virtual GitHubMarkdown GenerateRootMarkdown()
            {
            var MD = new GitHubMarkdown(this, this.MarkdownPath_Root, this.Language.MainReadme);
            this.WriteHeader(MD);
            MD.Header(this.Language.MainReadme, Size: 2);

            this.Home_Intro(MD);

            if (!this.GetType().GetMembers().Has(
                Member => Member.IsDeclaredMember() &&
                    (Member.Name == nameof(this.HowToInstall))))
                {
                MD.Header(this.Language.Header_InstallationInstructions, Size: 3);

                this.HowToInstall(MD);
                }

            this.Markdown_Assembly.Each(Document =>
            {
                var Coverage = new AssemblyCoverage(Document.Key);
                ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                MD.Header(Document.Value.Title, Size: 2);
                //MD.Line(this.GetBadges_Info(MD, Coverage, Comments).JoinLines(" "));
                MD.Line(this.GetBadges_Coverage(MD, Coverage, Comments).JoinLines(" "));

                MD.Line($" - {MD.Link(MD.GetRelativePath(Document.Value.FilePath), Document.Value.Title)}");
            });

            if (!this.Home_RelatedProjects.IsEmpty())
                MD.Header(this.Language.Header_RelatedProjects, Size: 3);

            MD.UnorderedList(
                this.Home_RelatedProjects.Convert(
                    Project => $"{MD.Link(Project.Url, Project.Name)} {Project.Description}").Array());


            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates table of contents document
        /// </summary>
        public virtual GitHubMarkdown GenerateTableOfContentsMarkdown()
            {
            var MD = new GitHubMarkdown(this, this.MarkdownPath_TableOfContents, this.Language.TableOfContents);

            this.WriteHeader(MD);

            MD.Header(this.Language.TableOfContents, Size: 2);

            this.GetAllMarkdown().Each(Document =>
                {
                    MD.Line($" - {MD.Link(MD.GetRelativePath(Document.FilePath), Document.Title)}");
                });

            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates coverage summary document
        /// </summary>
        public virtual GitHubMarkdown GenerateCoverageSummaryMarkdown()
            {
            var MD = new GitHubMarkdown(this, this.MarkdownPath_CoverageSummary, this.Language.CoverageSummary);

            this.WriteHeader(MD);

            MD.Header(this.Language.CoverageSummary);

            // TODO Generate summary markdown

            MD.Header(this.Language.Header_Assemblies);
            this.Markdown_Assembly.Each(AssemblyMD =>
                {
                    var Coverage = new AssemblyCoverage(AssemblyMD.Key);
                    ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

                    MD.Header(AssemblyMD.Value.Title, Size: 2);
                    //MD.Line(this.GetBadges_Info(MD, Coverage, Comments).JoinLines(" "));
                    MD.Line(this.GetBadges_Coverage(MD, Coverage, Comments).JoinLines(" "));

                });

            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates markdown for an Assembly
        /// </summary>
        public virtual GitHubMarkdown_Assembly GenerateMarkdown(Assembly Assembly)
            {
            return new GitHubMarkdown_Assembly(Assembly, this, this.MarkdownPath_Assembly(Assembly), Assembly.GetName().Name);
            }

        /// <summary>
        /// Generates markdown for a Type
        /// </summary>
        public virtual GitHubMarkdown_Type GenerateMarkdown(Type Type)
            {
            return new GitHubMarkdown_Type(Type, this, this.MarkdownPath_Type(Type), Type.Name);
            }

        /// <summary>
        /// Generates markdown for a group of Members
        /// </summary>
        public virtual GitHubMarkdown_MemberGroup GenerateMarkdown(MemberInfo[] MemberGroup)
            {
            var Member = MemberGroup.First();

            return new GitHubMarkdown_MemberGroup(MemberGroup, this, this.MarkdownPath_Member(Member), Member.Name);
            }

        #endregion

        #region Helpers +

        private string GetFrameworkVersion()
            {
            return this.GetType().Assembly.ImageRuntimeVersion;
            }

        /// <summary>
        /// Writes the default header, including the large or small
        /// banner and logo
        /// </summary>
        public virtual void WriteHeader(GitHubMarkdown MD)
            {
            bool IsMain = MD.FilePath == this.MarkdownPath_Root;


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
                MD.Line(MD.Link(MD.GetRelativePath(this.MarkdownPath_Root), MD.Image(this.LogoImage_Small(MD), this.Language.AltText_Logo, L.Align.Right)));
                }
            }


        /// <summary>
        /// Retrieves a formatted link to the table of contents
        /// </summary>
        public virtual string TableOfContentsLink(GitHubMarkdown MD)
            {
            return MD.Link(MD.GetRelativePath(this.MarkdownPath_TableOfContents), this.Language.TableOfContents);
            }

        /// <summary>
        /// Retrieves a formatted link to the home readme
        /// </summary>
        public virtual string HomeLink(GitHubMarkdown MD)
            {
            return MD.Link(MD.GetRelativePath(this.MarkdownPath_Root), this.Language.MainReadme);
            }

        /// <summary>
        /// Override this method to generate custom documents for your project.
        /// </summary>
        public virtual Dictionary<string, GitHubMarkdown> GetOtherDocuments()
            {
            return new Dictionary<string, GitHubMarkdown>();
            }

        /// <summary>
        /// Formats comments, properly displaying comment tags.
        /// </summary>
        public virtual string FormatComment(string CommentText)
            {
            // TODO add support for <see>
            // TODO add support for <seealso>

            return CommentText;
            }

        /// <summary>
        /// Gets a link to a type, whether it is public to this project, a type on GitHub,
        /// a type in related projects
        /// or a System type.
        /// 
        /// Otherwise, fall back on a google search.
        /// </summary>
        public virtual string LinkToType(GitHubMarkdown MD, Type Type)
            {
            // Unassigned types, (T, etc), need no link
            if (Type.IsGenericParameter)
                return Type.Name;

            // Ref types, link to actual type
            if (Type.IsByRef)
                return $"ref {this.LinkToType(MD, Type.GetElementType())}";

            // Array types, nest array symbols outside of the link
            if (Type.IsArray)
                return $"{this.LinkToType(MD, Type.GetElementType())}[]";

            // Generic types, display in standard generic notation and link each type individually. Recursion is required.
            if (Type.IsGenericType && !Type.IsGenericTypeDefinition)
                {
                string GenericTypeLink = this.LinkToType(MD, Type.GetGenericTypeDefinition());
                Type[] Parameters = Type.GenericTypeArguments;

                return $"{GenericTypeLink}&lt;{Parameters.Convert(Param => this.LinkToType(MD, Param)).Combine(", ")}&gt;";
                }


            // ---- From here on Type is known to not be generic, unknown generic parameter, ref, or array type ----

            string Name = Type.GetNestedNames().Before("<");

            // Local links for known documented types
            string TypeLink = this.Markdown_Type.First(MDType => MDType.Key == Type).Value?.FilePath;
            // bold local links
            if (!string.IsNullOrEmpty(TypeLink))
                return MD.Bold(MD.Link(MD.GetRelativePath(TypeLink), Name));


            // Resolve custom type links from implementation
            if (this.CustomTypeLinks.ContainsKey(Type))
                return MD.Link(this.CustomTypeLinks[Type], Name, "", TargetNewWindow: true);

            // Resolve default known type links
            if (ReferenceLinks.ContainsKey(Type))
                return MD.Link(ReferenceLinks[Type], Name, "", TargetNewWindow: true);

            // TODO: resolve related project assemblies

            // Resolve all non-generic System types
            if (!Type.ContainsGenericParameters && // Generic parameters aren't supported with named links on their docs
                Type.FullyQualifiedName().ToLower().StartsWith("system."))
                return MD.Link(MicrosoftSystemReferencePath(Type), Name);

            if (this.DocumentAssemblies.Has(Type.GetAssembly()))
                return MD.Link(MD.GetRelativePath(this.MarkdownPath_Type(Type)), Name);

            if (!this.TypeLinksNotFound.Has(Type))
                this.TypeLinksNotFound.Add(Type);

            return MD.Link("https://www.google.com/#q=C%23+" +
                            $"{WebUtility.HtmlEncode(Type.FullyQualifiedName())}",
                            Name,
                            $"Search for '{WebUtility.HtmlEncode(Type.FullyQualifiedName())}'",
                            TargetNewWindow: true);
            }

        /// <summary>
        /// Stores types where documentation wasn't successfully located
        /// </summary>
        protected List<Type> TypeLinksNotFound { get; } = new List<Type>();

        /// <summary>
        /// Get all Member group markdown owned by a given <paramref name="Type"/>
        /// </summary>
        public List<KeyValuePair<MemberInfo[], GitHubMarkdown_MemberGroup>> GetTypeMemberMarkdown(Type Type)
            {
            return this.Markdown_Member.Select(Member => Member.Key.First()?.DeclaringType?.Name == Type.Name);
            }

        /// <summary>
        /// Get all Type markdown for a given <paramref name="Assembly"/>
        /// </summary>
        public virtual List<KeyValuePair<Type, GitHubMarkdown_Type>> GetAssemblyTypeMarkdown(Assembly Assembly)
            {
            return this.Markdown_Type.Select(Type => Type.Key.GetAssembly()?.GetName().Name == Assembly.GetName().Name);
            }
        #endregion

        #region Badges +

        // TODO: project badges

        #region Assembly Badges

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Info([NotNull] GitHubMarkdown MD, [CanBeNull] AssemblyCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Assembly = Coverage?.CoveringAssembly;

            // ReSharper disable once UseObjectOrCollectionInitializer
            var Out = new List<string>();

            Out.Add(MD.Badge(this.Language.Badge_Framework,
                $"Version {this.GetFrameworkVersion()}",
                GitHubMarkdown.BadgeColor.Blue));

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
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Coverage([NotNull] GitHubMarkdown MD, [CanBeNull] AssemblyCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Assembly = Coverage?.CoveringAssembly;

            // ReSharper disable once UseObjectOrCollectionInitializer
            var Out = new List<string>();


            return Out;
            }

        #endregion

        // TODO: namespace info badges
        // TODO: namespace coverage badges

        #region Type Badges

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        [CanBeNull]
        public virtual List<string> GetBadges_Info([NotNull] GitHubMarkdown_Type MD, [CanBeNull] TypeCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Type = Coverage?.CoveringType;

            if (Type != null)
                {
                var Out = new List<string>();

                string TypeDescription =
                    Type.IsStatic() ? "Static Class" :
                        Type.IsAbstract ? "Abstract Class" :
                            Type.IsEnum ? "Enum " :
                                Type.IsInterface ? "Interface" :
                                    "Object Class";

                List<KeyValuePair<MemberInfo[], GitHubMarkdown_MemberGroup>> Members = this.GetTypeMemberMarkdown(Type);

                uint TotalDocumentable = 0;
                uint Documented = 0;

                // TODO: add total members
                // TODO: add total lines of code (non 'empty')
                // TODO: add total extension methods
                // TODO: add total todo count
                // TODO: add total bug count
                // TODO: add total not implemented count

                Members.Each(MemberGroup =>
                    {
                        MemberGroup.Key.Each(Member =>
                            {
                                TotalDocumentable++;

                                var MemberComments = Member.GetComments();

                                if (MemberComments != null)
                                    Documented++;
                            });
                    });


                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription, GitHubMarkdown.BadgeColor.Blue));

                if (TotalDocumentable > 0)
                    {
                    int DocumentedPercent = Documented.PercentageOf(TotalDocumentable);
                    Out.Add(MD.Badge(this.Language.Badge_Documented, $"{DocumentedPercent}%", this.GetColorByPercentage(DocumentedPercent)));
                    }

                return Out;
                }

            return null;
            }

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        [CanBeNull]
        public virtual List<string> GetBadges_Coverage([NotNull] GitHubMarkdown_Type MD, [CanBeNull] TypeCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Type = Coverage?.CoveringType;

            if (Type != null)
                {
                var Out = new List<string>();

                string TypeDescription =
                    Type.IsStatic() ? "Static Class" :
                        Type.IsAbstract ? "Abstract Class" :
                            Type.IsEnum ? "Enum " :
                                Type.IsInterface ? "Interface" :
                                    "Object Class";

                List<KeyValuePair<MemberInfo[], GitHubMarkdown_MemberGroup>> Members = this.GetTypeMemberMarkdown(Type);

                uint TotalCoverable = 0;
                uint Covered = 0;

                // TODO: add total lines of code (non 'empty')

                Members.Each(MemberGroup =>
                    {
                        MemberGroup.Key.Each(Member =>
                            {

                                var MemberComments = Member.GetComments();

                                if (Member is MethodInfo)
                                    {
                                    TotalCoverable++;

                                    var MemberCoverage = new MethodCoverage((MethodInfo)Member);

                                    if (MemberCoverage.IsCovered)
                                        Covered++;
                                    }
                            });
                    });


                if (TotalCoverable > 0)
                    {
                    int CoveredPercent = Covered.PercentageOf(TotalCoverable);
                    Out.Add(MD.Badge(this.Language.Badge_Covered, $"{CoveredPercent}%", this.GetColorByPercentage(CoveredPercent)));
                    }

                return Out;
                }

            return null;
            }

        #endregion

        // TODO: member group info badges
        // TODO: member group coverage badges

        #region Member Badges


        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Info([NotNull] GitHubMarkdown_MemberGroup MD, [CanBeNull] MethodCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Member = Coverage?.CoveringMember;

            var Out = new List<string>();

            if (Member != null)
                {
                string SourcePath = Member.DeclaringType?.FindClassFile();


                var Meta = MD.Members[Member];

                var TypeDescription = Member.GetMemberDetails();

                const GitHubMarkdown.BadgeColor InfoColor = GitHubMarkdown.BadgeColor.Blue;


                // Member Type
                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription.ToString(), InfoColor));

                // Lines of code
                Out.Add(MD.Badge(this.Language.Badge_LinesOfCode, $"{MD.Members.Sum(SubMember => SubMember.Value.CodeLineCount ?? 0u)}", InfoColor));

                // to-dos
                uint TODOCount = MD.Members.Sum(SubMember => SubMember.Value.CommentTODO.Length);
                Out.Add(MD.Badge(this.Language.Badge_TODOs, $"{TODOCount}", TODOCount > 0
                    ? GitHubMarkdown.BadgeColor.Yellow
                    : GitHubMarkdown.BadgeColor.Green));

                // bugs
                uint BugCount = MD.Members.Sum(SubMember => SubMember.Value.CommentBUG.Length);
                Out.Add(MD.Badge(this.Language.Badge_BUGs, $"{BugCount}", BugCount > 0
                    ? GitHubMarkdown.BadgeColor.Red
                    : GitHubMarkdown.BadgeColor.Green));

                uint NotImplementedCount = MD.Members.Sum(SubMember => SubMember.Value.NotImplemented.Length);
                Out.Add(MD.Badge(this.Language.Badge_NotImplemented, $"{NotImplementedCount}", NotImplementedCount > 0
                    ? GitHubMarkdown.BadgeColor.Orange
                    : GitHubMarkdown.BadgeColor.Green));

                // Documented
                Out.Add(MD.Badge(this.Language.Badge_Documented, Comments != null
                        ? "Yes" : "No",
                    Comments != null
                        ? GitHubMarkdown.BadgeColor.BrightGreen
                        : GitHubMarkdown.BadgeColor.Red));

                // Source code
                if (SourcePath == null)
                    Out.Add(MD.Badge(this.Language.Badge_SourceCode,
                        this.Language.Badge_SourceCodeUnavailable,
                        GitHubMarkdown.BadgeColor.Red));
                else
                    {
                    Out.Add(MD.Link($"{MD.GetRelativePath(SourcePath)}#L{Meta.CodeLineNumber}",
                        MD.Badge(this.Language.Badge_SourceCode,
                            this.Language.Badge_SourceCodeAvailable,
                            GitHubMarkdown.BadgeColor.BrightGreen), EscapeText: false));
                    }

                this.CustomCommentTags.Each(Tag =>
                {
                    Func<uint, GitHubMarkdown.BadgeColor> CommentColor = this.CustomCommentColor.SafeGet(Tag);
                    uint TagCount = MD.Members.Sum(SubMember => SubMember.Value.CommentTags[Tag].Length);

                    var Color = CommentColor?.Invoke(TagCount) ?? InfoColor;

                    Out.Add(MD.Badge(Tag, $"{TagCount}", Color));
                });
                }
            return Out;
            }


        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        public virtual List<string> GetBadges_Coverage([NotNull] GitHubMarkdown_MemberGroup MD, [CanBeNull] MethodCoverage Coverage,
            [CanBeNull] ICodeComment Comments)
            {
            var Member = Coverage?.CoveringMember;

            var Out = new List<string>();

            if (Member != null)
                {
                string SourcePath = Member.DeclaringType?.FindClassFile();

                string MethodScope = Member.IsPublic ? "Public" : "Protected";

                if (Member.IsAbstract)
                    MethodScope = $"Abstract {MethodScope}";

                string TypeDescription = Member.IsStatic ? "Static Method" : $"{MethodScope} Method";

                if (this.DocumentUnitCoverage)
                    Out.Add(MD.Badge(this.Language.Badge_UnitTested, Coverage.MemberTraitFound ? "Yes" : "No",
                        Coverage.MemberTraitFound
                            ? GitHubMarkdown.BadgeColor.BrightGreen
                            : GitHubMarkdown.BadgeColor.LightGrey));
                if (this.DocumentAttributeCoverage)
                    Out.Add(MD.Badge(this.Language.Badge_AttributeTests, $"{Coverage.AttributeCoverage}",
                        Coverage.AttributeCoverage > 0u
                            ? GitHubMarkdown.BadgeColor.BrightGreen
                            : GitHubMarkdown.BadgeColor.LightGrey));

                Out.Add(MD.Link(MD.GetRelativePath(SourcePath),
                    MD.Badge(this.Language.Badge_Assertions, $"{Coverage.AssertionsMade}",
                    Coverage.AssertionsMade > 0u
                    ? GitHubMarkdown.BadgeColor.BrightGreen
                    : GitHubMarkdown.BadgeColor.LightGrey), EscapeText: false));

                // TODO: Add Test Status: Passing / Failing / Untested
                }
            return Out;
            }


        #endregion

        #endregion

        #region Options +

        /// <summary>
        /// Requires all foreign types to have a link supplied.
        /// Override <see cref="CustomTypeLinks"/>
        /// </summary>
        public virtual bool RequireDirectLinksToAllForeignTypes => false;

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        public virtual string BannerImage_Large([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        public virtual string BannerImage_Small([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a large image in the upper right corner of the main document
        /// </summary>
        public virtual string LogoImage_Large([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a small image in the upper right corner of sub-documents
        /// </summary>
        public virtual string LogoImage_Small([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Writes the footer to a markdown document.
        /// </summary>
        public void WriteFooter([NotNull]GitHubMarkdown MD)
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
        public virtual void WriteCustomFooter(GitHubMarkdown MD)
            {
            MD.Line(new[]
                    {
                    $"Copyright {DateTime.Now.Year} &copy;",
                    this.HomeLink(MD),
                    this.TableOfContentsLink(MD)
                    }.JoinLines(" "));
            }


        /// <summary>
        /// Root path of the current running solution (development ONLY)
        /// </summary>
        public virtual string GeneratedMarkdownRoot => L.Ref.GetSolutionRootPath();


        /// <summary>
        /// Root readme full path
        /// </summary>
        public virtual string MarkdownPath_Root => $"{this.GeneratedMarkdownRoot}\\{MarkdownPath_RootFile}";

        /// <summary>
        /// Table of contents readme full path
        /// </summary>
        public virtual string MarkdownPath_TableOfContents
            => $"{this.GeneratedMarkdownRoot}\\{this.Language.TableOfContentsFile}";


        /// <summary>
        /// Coverage summary readme full path
        /// </summary>
        public virtual string MarkdownPath_CoverageSummary
            => $"{this.GeneratedMarkdownRoot}\\{this.Language.CoverageSummaryFile}";

        /// <summary>
        /// Documents folder, default is "docs"
        /// </summary>
        protected virtual string MarkdownPath_Documentation => "docs";

        /// <summary>
        /// Generates the document title for an Assembly
        /// </summary>
        public virtual string MarkdownPath_Assembly([NotNull]Assembly Assembly) =>
            $"{Assembly.GetRootPath()}\\{Assembly.GetName().Name.CleanFileName()}.md";

        /// <summary>
        /// Generates the document title for a Type
        /// </summary>
        public virtual string MarkdownPath_Type([NotNull]Type Type) =>
            $"{Type.Assembly.GetRootPath()}\\{this.MarkdownPath_Documentation}\\" +
            $"{Type.Name.CleanFileName()}.md";

        /// <summary>
        /// Generates the document title for a Member
        /// </summary>
        public virtual string MarkdownPath_Member([NotNull]MemberInfo Member) =>
            $"{Member.GetAssembly().GetRootPath()}\\{this.MarkdownPath_Documentation}\\" +
            $"{Member.DeclaringType?.Name.CleanFileName()}_{Member.Name.CleanFileName()}.md";

        /// <summary>
        /// Determines if a Type should be included in documentation
        /// </summary>
        public virtual bool IncludeType([NotNull]Type Type) =>
            !Type.HasAttribute<ExcludeFromCodeCoverageAttribute>(IncludeBaseClasses: true) &&
            !Type.HasAttribute<IExcludeFromMarkdownAttribute>();

        /// <summary>
        /// Determines if a Member should be included in documentation
        /// </summary>
        public virtual bool IncludeMember([NotNull]MemberInfo Member) =>
            !Member.HasAttribute<ExcludeFromCodeCoverageAttribute>(IncludeBaseClasses: true) &&
            !Member.HasAttribute<IExcludeFromMarkdownAttribute>() &&
            !(Member is MethodInfo && ((MethodInfo)Member).IsPropertyGetterOrSetter()) &&
            Member.IsDeclaredMember() &&
            !(Member is ConstructorInfo);

        /// <summary>
        /// Override this value with an array of EXACTLY 4 numbers to specify the levels of coloring
        /// Default is: `{ 30, 50, 70, 100 }`
        /// 
        /// Percentage  | Color
        /// ---         | ---
        /// 30-         | GitHuBMarkdown.BadgeColor.Red
        /// 31-50       | GitHuBMarkdown.BadgeColor.Yellow
        /// 51-70       | GitHuBMarkdown.BadgeColor.YellowGreen
        /// 71-100      | GitHuBMarkdown.BadgeColor.Green
        /// 100         | GitHuBMarkdown.BadgeColor.BrightGreen
        /// 101+        | GitHuBMarkdown.BadgeColor.Blue
        /// </summary>
        public virtual int[] ColorThresholds => new[] { 30, 50, 70, 100 };

        /// <summary>
        /// Gets a BadgeColor for a given <paramref name="Percentage"/>
        /// 
        /// Override this method to customize the deciding of BadgeColor by Percentage.
        /// </summary>
        public virtual GitHubMarkdown.BadgeColor GetColorByPercentage(int Percentage)
            {
            if (Percentage < this.ColorThresholds[0])
                return GitHubMarkdown.BadgeColor.Red;
            if (Percentage < this.ColorThresholds[1])
                return GitHubMarkdown.BadgeColor.Yellow;
            if (Percentage < this.ColorThresholds[2])
                return GitHubMarkdown.BadgeColor.YellowGreen;
            if (Percentage < this.ColorThresholds[3])
                return GitHubMarkdown.BadgeColor.Green;
            if (Percentage == this.ColorThresholds[3])
                return GitHubMarkdown.BadgeColor.BrightGreen;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (Percentage > this.ColorThresholds[3])
                return GitHubMarkdown.BadgeColor.Blue;

            return GitHubMarkdown.BadgeColor.LightGrey;
            }


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

        #region Private Methods +


        private void Load(Assembly Assembly)
            {
            Assembly.GetExportedTypes().Select(this.IncludeType).Each(this.Load);

            this.Markdown_Assembly.Add(Assembly, this.GenerateMarkdown(Assembly));
            }

        private void Load(Type Type)
            {
            Dictionary<string, List<MemberInfo>> MemberNames = Type.GetMembers()
                .Select(this.IncludeMember)
                .Group(Member => Member.Name);

            MemberNames.Values.Convert(EnumerableExt.Array).Each(this.Load);

            this.Markdown_Type.Add(Type, this.GenerateMarkdown(Type));
            }

        private void Load(MemberInfo[] MemberGroup)
            {
            this.Markdown_Member.Add(MemberGroup, this.GenerateMarkdown(MemberGroup));
            }

        #endregion

        #region Public Methods +

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

            // Lastly, generate table of contents
            this.Markdown_Other.Add(this.Language.TableOfContents, this.GenerateTableOfContentsMarkdown());

            List<GitHubMarkdown> AllMarkdown = this.GetAllMarkdown();

            if (!this.TypeLinksNotFound.IsEmpty())
                throw new InvalidOperationException(
                    $"Type links not found: {this.TypeLinksNotFound.Convert(Type => Type.FullyQualifiedName()).JoinLines(", ")}");

            if (WriteToDisk)
                {
                AllMarkdown.Each(MD =>
                    {
                        string Path = MD.FilePath;

                        // just to be safe
                        if (Path.EndsWith(".md"))
                            {
                            Path.EnsurePathExists();

                            File.WriteAllLines(Path, MD.GetMarkdownLines().Array());
                            }
                    });
                }
            }

        /// <summary>
        /// Gets all markdown generated by the generator.
        /// </summary>
        public List<GitHubMarkdown> GetAllMarkdown()
            {
            var AllMarkdown = new List<GitHubMarkdown>();

            AllMarkdown.AddRange(this.Markdown_Other.Values);
            AllMarkdown.AddRange(this.Markdown_Assembly.Values);
            AllMarkdown.AddRange(this.Markdown_Type.Values);
            AllMarkdown.AddRange(this.Markdown_Member.Values);

            return AllMarkdown;
            }

        #endregion

        #region Language +

        /// <summary>
        /// Override this value to customize the text used for markdown generation.
        /// </summary>
        public virtual Text Language => new Text
            {
            MainReadme = "Home",
            TableOfContents = "Table of Contents",
            CoverageSummary = "Coverage Summary",
            CoverageSummaryFile = "CoverageSummary.md",
            TableOfContentsFile = "TableOfContents.md",

            Header_Assemblies = "Assemblies",
            Header_InstallationInstructions = "Installation Instructions",
            Header_RelatedProjects = "Related Projects",
            Header_MethodExamples = "Examples",
            Header_Summary = "Summary",
            Header_MethodParameters = "Parameters",
            Header_MethodReturns = "Returns",
            Header_MethodPermissions = "Permissions",
            Header_MethodExceptions = "Exceptions",
            Header_CodeLines = "Lines of Code",
            Header_Coverage = "Coverage",
            Header_Documentation = "Documented",


            LinkText_ViewSource = "View Source",
            LinkText_Home = "Home",
            LinkText_Up = "Up",

            TableHeaderText_MethodParameter = "Parameter",
            TableHeaderText_Optional = "Optional",
            TableHeaderText_Type = "Type",
            TableHeaderText_Description = "Description",

            Badge_Type = "Type",
            Badge_Documented = "Documented",
            Badge_Assertions = "Assertions",
            Badge_AttributeTests = "AttributeTests",
            Badge_Covered = "Covered",
            Badge_Framework = "Framework",
            Badge_SourceCode = "SourceCode",
            Badge_SourceCodeAvailable = "Available",
            Badge_SourceCodeUnavailable = "Unavailable",
            Badge_UnitTested = "UnitTested",
            Badge_LinesOfCode = "Lines of Code",
            Badge_BUGs = "Bugs",
            Badge_TODOs = "TODOs",
            Badge_NotImplemented = "Not Implemented",

            AltText_Logo = "Logo"
            };

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
        public virtual Dictionary<string, Func<uint, GitHubMarkdown.BadgeColor>> CustomCommentColor => new Dictionary<string, Func<uint, GitHubMarkdown.BadgeColor>>();

        /// <summary>
        /// Structure to customize text used in MarkdownGenerator
        /// </summary>
        public struct Text
            {
            /// <summary>
            /// Main readme title, default is "Home"
            /// </summary>
            public string MainReadme { get; set; }

            /// <summary>
            /// Table of Contents readme title, default is "Table of Contents"
            /// </summary>
            public string TableOfContents { get; set; }

            /// <summary>
            /// Coverage Summary readme title, default is "Coverage Summary"
            /// </summary>
            public string CoverageSummary { get; set; }

            /// <summary>
            /// Coverage summary file name, default is "CoverageSummary.md"
            /// </summary>
            public string CoverageSummaryFile { get; set; }

            /// <summary>
            /// Table of contents file name, default is "TableOfContents.md"
            /// </summary>
            public string TableOfContentsFile { get; set; }

            /// <summary>
            /// Badge title for project framework
            /// </summary>
            public string Badge_Framework { get; set; }
            /// <summary>
            /// Badge title for object Type
            /// </summary>
            public string Badge_Type { get; set; }
            /// <summary>
            /// Badge title for lines of code
            /// </summary>
            public string Badge_LinesOfCode { get; set; }
            /// <summary>
            /// Badge title for to-dos
            /// </summary>
            public string Badge_TODOs { get; set; }
            /// <summary>
            /// Badge title for bugs
            /// </summary>
            public string Badge_BUGs { get; set; }
            /// <summary>
            /// Badge title for NotImplementedException
            /// </summary>
            public string Badge_NotImplemented { get; set; }


            /// <summary>
            /// Badge title for member Documented
            /// </summary>
            public string Badge_Documented { get; set; }
            /// <summary>
            /// Badge title for member Assertions
            /// </summary>
            public string Badge_Assertions { get; set; }

            /// <summary>
            /// Badge title for member UnitTested
            /// </summary>
            public string Badge_UnitTested { get; set; }
            /// <summary>
            /// Badge title for member Covered
            /// </summary>
            public string Badge_Covered { get; set; }
            /// <summary>
            /// Badge title for member Source Code
            /// </summary>
            public string Badge_SourceCode { get; set; }
            /// <summary>
            /// Badge title for member Source Code Available
            /// </summary>
            public string Badge_SourceCodeAvailable { get; set; }
            /// <summary>
            /// Badge title for member Source Code Unavailable
            /// </summary>
            public string Badge_SourceCodeUnavailable { get; set; }
            /// <summary>
            /// Badge title for member AttributeTests
            /// </summary>
            public string Badge_AttributeTests { get; set; }

            /// <summary>
            /// Badge title for Assemblies Header
            /// </summary>
            public string Header_Assemblies { get; set; }
            /// <summary>
            /// Badge title for Installation Instructions Header
            /// </summary>
            public string Header_InstallationInstructions { get; set; }

            /// <summary>
            /// Header for related projects
            /// </summary>
            public string Header_RelatedProjects { get; set; }

            /// <summary>
            /// Header for method parameters
            /// </summary>
            public string Header_MethodParameters { get; set; }

            /// <summary>
            /// Header for method returns
            /// </summary>
            public string Header_MethodReturns { get; set; }
            /// <summary>
            /// Header for method summary
            /// </summary>
            public string Header_Summary { get; set; }

            /// <summary>
            /// Header for method examples
            /// </summary>
            public string Header_MethodExamples { get; set; }

            /// <summary>
            /// Header for method permissions
            /// </summary>
            public string Header_MethodPermissions { get; set; }

            /// <summary>
            /// Header for method exceptions
            /// </summary>
            public string Header_MethodExceptions { get; set; }

            /// <summary>
            /// Header for code lines
            /// </summary>
            public string Header_CodeLines { get; set; }
            /// <summary>
            /// Header for documentation
            /// </summary>
            public string Header_Documentation { get; set; }
            /// <summary>
            /// Header for coverage
            /// </summary>
            public string Header_Coverage { get; set; }


            /// <summary>
            /// Link text for view source
            /// </summary>
            public string LinkText_ViewSource { get; set; }
            /// <summary>
            /// Link text for home
            /// </summary>
            public string LinkText_Home { get; set; }
            /// <summary>
            /// Link text for up
            /// </summary>
            public string LinkText_Up { get; set; }

            /// <summary>
            /// Alt text for the logo
            /// </summary>
            public string AltText_Logo { get; set; }

            /// <summary>
            /// Table header text method parameter
            /// </summary>
            public string TableHeaderText_MethodParameter { get; set; }

            /// <summary>
            /// Table header text optional
            /// </summary>
            public string TableHeaderText_Optional { get; set; }

            /// <summary>
            /// Table header text type
            /// </summary>
            public string TableHeaderText_Type { get; set; }
            /// <summary>
            /// Table header text description
            /// </summary>
            public string TableHeaderText_Description { get; set; }
            }

        #endregion

        }
    }