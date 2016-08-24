using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LDoc;
using LCore.LDoc.Markdown;

namespace Test_LDoc
    {
    public class LDocMarkdownGenerator : MarkdownGenerator
        {
        protected override Assembly[] DocumentAssemblies => new[] { Assembly.GetAssembly(typeof(LDoc)) };

        protected override void Home_Intro(GitHubMarkdown MD)
            {

            }

        protected override void HowToInstall(GitHubMarkdown MD)
            {
            MD.Line($"Add {nameof(LCore.LDoc)} as a nuget package:");
            MD.Code(new[] { $"Install-Package {nameof(LCore.LDoc)}" });
            }

        /// <summary>
        /// Override this value to display a large image on top ofthe main document
        /// </summary>
        protected override string BannerImage_Large(GitHubMarkdown MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-large.png");

        /// <summary>
        /// Override this value to display a small banner image on top of sub-documents
        /// </summary>
        protected override string BannerImage_Small(GitHubMarkdown MD) =>
            MD.GetRelativePath($"{typeof(LDoc).GetAssembly().GetRootPath()}\\Content\\{nameof(LDoc)}-banner-small.png");

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