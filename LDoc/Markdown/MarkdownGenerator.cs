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
// ReSharper disable VirtualMemberNeverOverridden.Global

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
        /// Writes the footer for your markdown document
        /// </summary>
        public virtual void WriteCustomFooter(GitHubMarkdown MD)
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
        /// Gets a link to a type, whether it is public to this project, a type on GitHub,
        /// a type in related projects
        /// or a System type.
        /// 
        /// Otherwise, fall back on a google search.
        /// </summary>
        public virtual string GetTypeLink(GitHubMarkdown MD, Type Type)
            {
            // TODO: resolve local types

            // TODO: resolve github types

            // TODO: resolve microsoft-documented types

            // TODO: google unknown types

            return "";
            }

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

                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;

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


                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription));

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

                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;

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


                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription));

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

                string MethodScope = Member.IsPublic ? "Public" : "Protected";

                var Meta = MD.Members[Member];

                if (Member.IsAbstract)
                    MethodScope = $"Abstract {MethodScope}";

                string TypeDescription = Member.IsStatic ? "Static Method" : $"{MethodScope} Method";
                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;


                // Member Type
                Out.Add(MD.Badge(this.Language.Badge_Type, TypeDescription));

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
                            GitHubMarkdown.BadgeColor.BrightGreen)));
                    }
                // TODO: add total lines of code (non 'empty')
                // TODO: add total todo count
                // TODO: add total bug count
                // TODO: add total not implemented count

                // TODO custom flag tracking
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
                const GitHubMarkdown.BadgeColor TypeColor = GitHubMarkdown.BadgeColor.LightGrey;


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
                    MD.Badge(this.Language.Badge_Assertions, $"{Coverage.AssertionsMade}", GitHubMarkdown.BadgeColor.BrightGreen)));

                // TODO: Add Test Status: Passing / Failing / Untested
                }
            return Out;
            }


        #endregion

        #endregion

        #region Options +

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

            public string Header_MethodPermissions { get; set; }
            public string Header_MethodExceptions { get; set; }



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