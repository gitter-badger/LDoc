using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LDoc;
using LCore.LDoc.Markdown;
using LCore.LUnit;

namespace Test_LDoc
    {
    public class LDocMarkdownGenerator : MarkdownGenerator
        {
        protected override Assembly[] DocumentAssemblies => new[] { Assembly.GetAssembly(typeof(LDoc)) };

        /// <summary>
        /// Override this value to indicate installation instructions.
        /// </summary>
        protected override string HowToInstall_Text(GitHubMarkdown MD) => $"Add {nameof(LCore.LUnit)} as a nuget package:";

        /// <summary>
        /// Override this value to indicate installation instructions.
        /// This text will be formatted as C# code below <see cref="MarkdownGenerator.HowToInstall_Text"/>
        /// </summary>
        protected override string HowToInstall_Code(GitHubMarkdown MD) => $"Install-Package {nameof(LCore.LDoc)}";

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