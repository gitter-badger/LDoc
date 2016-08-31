using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LDoc;
using LCore.LDoc.Markdown;
using LCore.LUnit;
using LCore.Tools;

namespace Test_LDoc
    {
    public class LDocSolutionMarkdownGenerator : SolutionMarkdownGenerator_L
        {
        public override Assembly[] DocumentAssemblies => new[] { Assembly.GetAssembly(typeof(LDoc)) };

        public override void Home_Intro(GeneratedDocument MD)
            {
            }

        public override void HowToInstall(GeneratedDocument MD)
            {
            MD.Line($"Add {nameof(LCore.LDoc)} as a nuget package:");
            MD.Code(new[] { $"Install-Package {nameof(LCore.LDoc)}" });
            }

        public override List<ProjectInfo> Home_RelatedProjects
            => base.Home_RelatedProjects.Select(Project => Project.Name != nameof(LDoc));

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        public override string BannerImage_Large(GeneratedDocument MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-large.png");

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        public override string BannerImage_Small(GeneratedDocument MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-small.png");

        public override bool RequireDirectLinksToAllForeignTypes => true;

        private const string RootLUnitGitHub = "https://github.com/CodeSingularity/LUnit/blob/master";

        public override Dictionary<Type, string> CustomTypeLinks => new Dictionary<Type, string>
            {
            [typeof(AssemblyCoverage)] = $"{RootLUnitGitHub}/LUnit/docs/AssemblyCoverage.md",
            [typeof(BadgeColor)] = "", // TODO link enums properly, fix in LCore find source file for enum types

            [typeof(ICodeComment)] = "", // TODO link once LCore is documented
            [typeof(L.Align)] = "", // TODO link once LCore is documented

            [typeof(Set<,>)] = "",
            [typeof(TypeCoverage)] = ""
            };

        /*
        /// <summary>
        /// Override this value to display a large image in the upper right corner of the main document
        /// </summary>
        protected override string LogoImage_Large(GitHubMarkdown MD) =>
        MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-logo-small.png");

        /// <summary>
        /// Override this value to display a small image in the upper right corner of sub-documents
        /// </summary>
        protected override string LogoImage_Small(GitHubMarkdown MD) =>
        MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-logo-small.png");*/
        }
    }