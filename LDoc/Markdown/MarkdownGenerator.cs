using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LUnit;
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedVariable

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
        /// Override this member to specify the assemblies to generae documentation.
        /// </summary>
        protected abstract Assembly[] DocumentAssemblies { get; }

        /// <summary>
        /// Write the markdown intro to your project, in the front page README.
        /// </summary>
        protected abstract void Home_Intro(GitHubMarkdown MD);

        public virtual List<ProjectInfo> Home_RelatedProjects { get; }

        /// <summary>
        /// Override this value to indicate installation instructions.
        /// </summary>
        protected virtual void HowToInstall([NotNull] GitHubMarkdown MD)
            {

            }

        #region Variables + 

        /// <summary>
        /// Other titled markdown,
        /// Root readme, table of contents, coverage summary, 
        /// custom documents, etc.
        /// </summary>
        protected Dictionary<string, GitHubMarkdown> Markdown_Other { get; } = new Dictionary<string, GitHubMarkdown>();

        /// <summary>
        /// Assembly-generated markdown documents.
        /// </summary>
        protected Dictionary<Assembly, GitHubMarkdown_Assembly> Markdown_Assembly { get; } =
            new Dictionary<Assembly, GitHubMarkdown_Assembly>();

        /// <summary>
        /// Type-generated markdown documents.
        /// </summary>
        protected Dictionary<Type, GitHubMarkdown_Type> Markdown_Type { get; } = new Dictionary<Type, GitHubMarkdown_Type>();

        /// <summary>
        /// Member-generated markdown documents.
        /// </summary>
        protected Dictionary<MemberInfo[], GitHubMarkdown_Member> Markdown_Member { get; } =
            new Dictionary<MemberInfo[], GitHubMarkdown_Member>();


        #endregion

        #region Generators + 

        /// <summary>
        /// Generates root markdown document (front page)
        /// </summary>
        protected virtual GitHubMarkdown GenerateRootMarkdown()
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
                MD.Line(this.GetBadges(MD, Coverage, Comments).JoinLines(" "));

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
        protected virtual GitHubMarkdown GenerateTableOfContentsMarkdown()
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
        protected virtual GitHubMarkdown GenerateCoverageSummaryMarkdown()
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
                    MD.Line(this.GetBadges(MD, Coverage, Comments).JoinLines(" "));

                });

            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates markdown for an Assembly
        /// </summary>
        protected virtual GitHubMarkdown_Assembly GenerateMarkdown(Assembly Assembly)
            {
            var MD = new GitHubMarkdown_Assembly(Assembly, this, this.MarkdownPath_Assembly(Assembly), Assembly.GetName().Name);

            this.WriteHeader(MD);

            var Coverage = new AssemblyCoverage(Assembly);
            ICodeComment Comments = null; // No assembly comments Document.Key.GetComments();

            MD.Line(MD.Link(MD.GetRelativePath(this.MarkdownPath_Root), this.Language.LinkText_Home));

            MD.Header($"{Assembly.GetName().Name}", Size: 2);
            MD.Line(this.GetBadges(MD, Coverage, Comments).JoinLines(" "));

            // TODO add Assembly comments

            // TODO sort by class type

            this.Markdown_Type.Select(Type => Type.Key.GetAssembly()?.GetName().Name == Assembly.GetName().Name)
                .Each(MD2 =>
                {
                    MD.Line($" - {MD.Link(MD.GetRelativePath(this.MarkdownPath_Type(MD2.Key)), MD2.Key.Name)}");
                });

            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates markdown for a Type
        /// </summary>
        protected virtual GitHubMarkdown_Type GenerateMarkdown(Type Type)
            {
            var MD = new GitHubMarkdown_Type(Type, this, this.MarkdownPath_Type(Type), Type.Name);

            this.WriteHeader(MD);

            var Coverage = new TypeCoverage(Type);
            var Comments = Type.GetComments();

            MD.Line(MD.Link(MD.GetRelativePath(this.MarkdownPath_Assembly(Type.GetAssembly())), this.Language.LinkText_Up));

            MD.Header($"{Type.Name}", Size: 3);
            MD.Line(this.GetBadges(MD, Coverage, Comments).JoinLines(" "));

            string TypePath = Type.FindClassFile();

            if (!string.IsNullOrEmpty(TypePath))
                {
                MD.Line(MD.Link(MD.GetRelativePath(TypePath), this.Language.LinkText_ViewSource));
                }

            if (!string.IsNullOrEmpty(Comments?.Summary))
                {
                MD.Header(this.Language.Header_Summary, Size: 6);
                MD.Line(Comments.Summary);
                }

            this.GetTypeMemberMarkdown(Type).Each(Member =>
                MD.Line($" - {MD.Link(MD.GetRelativePath(this.MarkdownPath_Member(Member.Key.First())), $"{Member.Key.First()?.Name}")}"));

            this.WriteFooter(MD);

            return MD;
            }

        /// <summary>
        /// Generates markdown for a group of Members
        /// </summary>
        protected virtual GitHubMarkdown_Member GenerateMarkdown(MemberInfo[] MemberGroup)
            {
            var Member = MemberGroup.First();

            var MD = new GitHubMarkdown_Member(MemberGroup, this, this.MarkdownPath_Member(Member), Member.Name);

            this.WriteHeader(MD);

            MD.Line(MD.Link(MD.GetRelativePath(this.MarkdownPath_Type(Member.DeclaringType)), this.Language.LinkText_Up));

            MD.Header($"{Member.DeclaringType?.Name}", Size: 3);

            string TypePath = Member.DeclaringType.FindClassFile();

            if (!string.IsNullOrEmpty(TypePath))
                {
                MD.Line(MD.Link(MD.GetRelativePath(TypePath), this.Language.LinkText_ViewSource));
                }


            MD.Header(Member.Name);

            if (Member is MethodInfo)
                {
                var Method = (MethodInfo)Member;
                var Coverage = new MethodCoverage(Method);
                var Comments = Method.GetComments();

                string Static = Method.IsStatic
                    ? "Static "
                    : "Instance";

                string StaticLower = Method.IsStatic
                    ? " static"
                    : "";

                string ReturnType = MD.Link(this.GetTypeLink(MD, Method.ReturnType), $"{Method.ReturnType.GetGenericName()}");


                string Parameters = Method.GetParameters()
                    .Convert(Param => $"{MD.Link(this.GetTypeLink(MD, Param.ParameterType), $"{Param.ParameterType.GetGenericName()}")} {Param.Name}")
                    .Combine(", ");

                List<string> Badges = this.GetBadges(MD, Coverage, Comments);

                MD.Line("");
                MD.Line(Badges.JoinLines(" "));
                MD.Line("");

                MD.Header($"{Static}Method", Size: 4);


                MD.Header($"public{StaticLower} {ReturnType} {Member.Name}({Parameters});", Size: 6);

                MD.Header(this.Language.Header_Summary, Size: 6);
                MD.Line(Comments?.Summary);

                if (Method.GetParameters().Length > 0)
                    {
                    MD.Header(this.Language.Header_MethodParameters, Size: 6);

                    var Table = new List<string[]>
                        {
                        new[] {this.Language.TableHeaderText_MethodParameter,
                            this.Language.TableHeaderText_Optional,
                            this.Language.TableHeaderText_Type,
                            this.Language.TableHeaderText_Description}
                        };

                    Method.GetParameters().Each((ParamIndex, Param) =>
                    {
                        Table.Add(new[]
                            {
                            Param.Name,
                            Param.IsOptional
                                ? "Yes"
                                : "No",
                            MD.Link(this.GetTypeLink(MD, Param.ParameterType), Param.ParameterType.GetGenericName()),
                            Comments?.Parameters.GetAt(ParamIndex)?.Obj2
                            });
                    });

                    MD.Table(Table);
                    }

                MD.Header(this.Language.Header_MethodReturns, Size: 4);

                MD.Header(MD.Link(this.GetTypeLink(MD, Method.ReturnType), Method.ReturnType.GetGenericName()), Size: 6);

                MD.Line(Comments?.Returns);

                if (Comments?.Examples.Length > 0)
                    {
                    MD.Header(this.Language.Header_MethodExamples, Size: 4);
                    Comments.Examples.Each(Example => MD.Code(new[] { Example }));
                    }

                // TODO: Add test coverage link

                // TODO: Add exception details

                // TODO: Add permission details
                }

            this.WriteFooter(MD);

            return MD;
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
        protected virtual void WriteHeader(GitHubMarkdown MD)
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
        /// Writes the footer for your markdown document
        /// </summary>
        protected virtual void WriteCustomFooter(GitHubMarkdown MD)
            {
            // TODO: Add custom copyright

            MD.Table(new[]
                {
                new[]
                    {
                    this.HomeLink(MD),
                    this.TableOfContentsLink(MD),
                    ""
                    }
                }, IncludeHeader: false);
            }

        /// <summary>
        /// Retrieves a formatted link to the table of contents
        /// </summary>
        protected virtual string TableOfContentsLink(GitHubMarkdown MD)
            {
            return MD.Link(MD.GetRelativePath(this.MarkdownPath_TableOfContents), this.Language.TableOfContents);
            }

        /// <summary>
        /// Retrieves a formatted link to the home readme
        /// </summary>
        protected virtual string HomeLink(GitHubMarkdown MD)
            {
            return MD.Link(MD.GetRelativePath(this.MarkdownPath_Root), this.Language.MainReadme);
            }

        /// <summary>
        /// Override this method to generate custom documents for your project.
        /// </summary>
        protected virtual Dictionary<string, GitHubMarkdown> GetOtherDocuments()
            {
            return new Dictionary<string, GitHubMarkdown>();
            }

        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        protected virtual List<string> GetBadges([NotNull] GitHubMarkdown MD, [CanBeNull] AssemblyCoverage Coverage,
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
            // TODO: add total lines of code (non 'empty')
            // TODO: add total extension methods

            return Out;
            }


        /// <summary>
        /// Override this method to customize badges included in type generated markdown documents.
        /// </summary>
        [CanBeNull]
        protected virtual List<string> GetBadges([NotNull] GitHubMarkdown MD, [CanBeNull] TypeCoverage Coverage,
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

                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;

                List<KeyValuePair<MemberInfo[], GitHubMarkdown_Member>> Members = this.GetTypeMemberMarkdown(Type);

                uint TotalCoverable = 0;
                uint Covered = 0;
                uint TotalDocumentable = 0;
                uint Documented = 0;

                // TODO: add total lines of code (non 'empty')

                Members.Each(MemberGroup =>
                    {
                        MemberGroup.Key.Each(Member =>
                            {
                                TotalDocumentable++;

                                var MemberComments = Member.GetComments();

                                if (MemberComments != null)
                                    Documented++;

                                if (Member is MethodInfo)
                                    {
                                    TotalCoverable++;

                                    var MemberCoverage = new MethodCoverage((MethodInfo)Member);

                                    if (MemberCoverage.IsCovered)
                                        Covered++;
                                    }
                            });
                    });


                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription));

                if (TotalDocumentable > 0)
                    {
                    int DocumentedPercent = Documented.PercentageOf(TotalDocumentable);
                    Out.Add(MD.Badge(this.Language.Badge_Documented, $"{DocumentedPercent}%", this.GetColorByPercentage(DocumentedPercent)));
                    }

                if (TotalCoverable > 0)
                    {
                    int CoveredPercent = Covered.PercentageOf(TotalCoverable);
                    Out.Add(MD.Badge(this.Language.Badge_Covered, $"{CoveredPercent}%", this.GetColorByPercentage(CoveredPercent)));
                    }

                return Out;
                }

            return null;
            }

        /// <summary>
        /// Override this method to customize badges included in member generated markdown documents.
        /// </summary>
        protected virtual List<string> GetBadges([NotNull] GitHubMarkdown MD, [CanBeNull] MethodCoverage Coverage,
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
                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;


                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription));
                Out.Add(MD.Badge(this.Language.Badge_Documented, Comments != null ? "Yes" : "No",
                    Comments != null ? GitHubMarkdown.BadgeColor.BrightGreen : GitHubMarkdown.BadgeColor.Red));
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

                if (SourcePath == null)
                    Out.Add(MD.Badge(this.Language.Badge_SourceCode, this.Language.Badge_SourceCodeUnavailable, GitHubMarkdown.BadgeColor.Red));
                else
                    Out.Add(MD.Link(MD.GetRelativePath(SourcePath),
                        MD.Badge(this.Language.Badge_SourceCode, this.Language.Badge_SourceCodeAvailable, GitHubMarkdown.BadgeColor.BrightGreen)));

                // TODO: add total lines of code (non 'empty')

                Out.Add(MD.Link(MD.GetRelativePath(SourcePath),
                    MD.Badge(this.Language.Badge_Assertions, $"{Coverage.AssertionsMade}", GitHubMarkdown.BadgeColor.BrightGreen)));

                // TODO: Add Test Status: Passing / Failing / Untested
                }
            return Out;
            }

        protected virtual string GetTypeLink(GitHubMarkdown MD, Type Type)
            {
            // TODO: resolve local types

            // TODO: resolve github types

            // TODO: resolve microsoft-documented types

            // TODO: google unknown types

            return "";
            }

        #endregion

        #region Options +

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        protected virtual string BannerImage_Large([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        protected virtual string BannerImage_Small([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a large image in the upper right corner of the main document
        /// </summary>
        protected virtual string LogoImage_Large([NotNull]GitHubMarkdown MD) => "";

        /// <summary>
        /// Override this value to display a small image in the upper right corner of sub-documents
        /// </summary>
        protected virtual string LogoImage_Small([NotNull]GitHubMarkdown MD) => "";



        private void WriteFooter([NotNull]GitHubMarkdown MD)
            {
            MD.HorizontalRule();

            // TODO: Add custom copyright

            this.WriteCustomFooter(MD);
            MD.Table(new[]
                {
                new[]
                    {
                    $"This markdown was generated by {MD.Link(LDoc.Urls.GitHubUrl, nameof(LDoc))}, " +
                    $"powered by {MD.Link(LUnit.LUnit.Urls.GitHubRepository_LUnit, nameof(LUnit))}, " +
                    $"{MD.Link(LUnit.LUnit.Urls.GitHubRepository_LCore, nameof(LCore))}",
                    ""
                    }
                }, IncludeHeader: false);
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
        protected virtual bool IncludeType([NotNull]Type Type) =>
            !Type.HasAttribute<ExcludeFromCodeCoverageAttribute>(IncludeBaseClasses: true) &&
            !Type.HasAttribute<IExcludeFromMarkdownAttribute>();

        /// <summary>
        /// Determines if a Member should be included in documentation
        /// </summary>
        protected virtual bool IncludeMember([NotNull]MemberInfo Member) =>
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
        protected virtual int[] ColorThresholds => new[] { 30, 50, 70, 100 };

        /// <summary>
        /// Gets a BadgeColor for a given <paramref name="Percentage"/>
        /// 
        /// Override this method to customize the deciding of BadgeColor by Percentage.
        /// </summary>
        protected virtual GitHubMarkdown.BadgeColor GetColorByPercentage(int Percentage)
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
        protected virtual bool DocumentUnitCoverage => true;

        /// <summary>
        /// Override this value to disable LUnit Attribute test coverage tracking.
        /// Default is true.
        /// </summary>
        protected virtual bool DocumentAttributeCoverage => true;
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


        private List<KeyValuePair<MemberInfo[], GitHubMarkdown_Member>> GetTypeMemberMarkdown(Type Type)
            {
            return this.Markdown_Member.Select(Member => Member.Key.First()?.DeclaringType?.Name == Type.Name);
            }

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

            AltText_Logo = "Logo"
            };

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

            public string Badge_Framework { get; set; }
            public string Badge_Type { get; set; }
            public string Badge_Documented { get; set; }
            public string Badge_Assertions { get; set; }
            public string Badge_UnitTested { get; set; }
            public string Badge_Covered { get; set; }
            public string Badge_SourceCode { get; set; }
            public string Badge_SourceCodeAvailable { get; set; }
            public string Badge_SourceCodeUnavailable { get; set; }
            public string Badge_AttributeTests { get; set; }

            public string Header_Assemblies { get; set; }
            public string Header_InstallationInstructions { get; set; }
            public string Header_RelatedProjects { get; set; }
            public string Header_MethodParameters { get; set; }
            public string Header_MethodReturns { get; set; }
            public string Header_Summary { get; set; }
            public string Header_MethodExamples { get; set; }

            public string LinkText_ViewSource { get; set; }
            public string LinkText_Home { get; set; }
            public string LinkText_Up { get; set; }

            public string AltText_Logo { get; set; }

            public string TableHeaderText_MethodParameter { get; set; }
            public string TableHeaderText_Optional { get; set; }
            public string TableHeaderText_Type { get; set; }
            public string TableHeaderText_Description { get; set; }
            }
        }
    }