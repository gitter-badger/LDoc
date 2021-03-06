using System;
using System.Collections;
using System.Collections.Generic;

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Markdown generator for projects in the L family.
    /// </summary>
    public abstract class SolutionMarkdownGenerator_L : SolutionMarkdownGenerator
        {
        /// <inheritdoc />
        public override List<ProjectInfo> Home_RelatedProjects => new List<ProjectInfo>
            {
            new ProjectInfo
                {
                Name = "LCore",
                Description = "",
                Url = LUnit.LUnit.Urls.GitHubRepository_LCore,
                LDocTypeManifestUrls = new [] { "https://raw.githubusercontent.com/CodeSingularity/LCore/master/type-manifest.json" }
                },
            new ProjectInfo
                {
                Name = "LUnit",
                Description = "",
                Url = LUnit.LUnit.Urls.GitHubRepository_LUnit,
                LDocTypeManifestUrls = new [] { "https://raw.githubusercontent.com/CodeSingularity/LUnit/master/type-manifest.json" }
                },
            new ProjectInfo
                {
                Name = "LDoc",
                Description = "",
                Url = LDoc.Urls.GitHubUrl,
                LDocTypeManifestUrls = new[] { "https://raw.githubusercontent.com/CodeSingularity/LDoc/master/type-manifest.json" }
                }
            };
        }
    }