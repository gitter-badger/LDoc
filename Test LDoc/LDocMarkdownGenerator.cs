using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.Interfaces;
using LCore.LDoc;
using LCore.LDoc.Markdown;
using LCore.LUnit;

namespace Test_LDoc
    {
    public class LDocMarkdownGenerator : MarkdownGenerator
        {
        public override Assembly[] DocumentAssemblies => new[] { Assembly.GetAssembly(typeof(LDoc)) };

        public override void Home_Intro(GitHubMarkdown MD)
            {

            }

        public override void HowToInstall(GitHubMarkdown MD)
            {
            MD.Line($"Add {nameof(LCore.LDoc)} as a nuget package:");
            MD.Code(new[] { $"Install-Package {nameof(LCore.LDoc)}" });
            }

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        public override string BannerImage_Large(GitHubMarkdown MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-large.png");

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        public override string BannerImage_Small(GitHubMarkdown MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-small.png");

        public override bool RequireDirectLinksToAllForeignTypes => true;

        private const string RootLUnitGitHub = "https://github.com/CodeSingularity/LUnit/blob/master";

        public override Dictionary<Type, string> CustomTypeLinks => new Dictionary<Type, string>
            {
            [typeof(List<>)] = "https://msdn.microsoft.com/en-us/library/6sh2ey19.aspx",
            [typeof(string)] = "https://msdn.microsoft.com/en-us/library/system.string.aspx",
            [typeof(void)] = "https://msdn.microsoft.com/en-us/library/system.void.aspx",
            [typeof(int)] = "https://msdn.microsoft.com/en-us/library/system.int32.aspx",
            [typeof(bool)] = "https://msdn.microsoft.com/en-us/library/system.boolean.aspx",
            [typeof(Nullable<>)] = "https://msdn.microsoft.com/en-us/library/system.nullable.aspx",
            [typeof(Assembly)] = "https://msdn.microsoft.com/en-us/library/system.reflection.assembly.aspx",
            [typeof(Type)] = "https://msdn.microsoft.com/en-us/library/system.type.aspx",
            [typeof(MemberInfo)] = "https://msdn.microsoft.com/en-us/library/system.reflection.memberinfo.aspx",
            [typeof(Dictionary<,>)] = "https://msdn.microsoft.com/en-us/library/xfhwa508.aspx",
            [typeof(KeyValuePair<,>)] = "https://msdn.microsoft.com/en-us/library/5tbh8a42.aspx",

            [typeof(AssemblyCoverage)] = $"{RootLUnitGitHub}/LUnit/docs/AssemblyCoverage.md",

            [typeof(GitHubMarkdown.BadgeColor)] = "", // TODO link enums properly, fix in LCore find source file for enum types

            [typeof(ICodeComment)] = "", // TODO link once LCode is documented
            [typeof(L.Align)] = ""      // TODO link once LCode is documented
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