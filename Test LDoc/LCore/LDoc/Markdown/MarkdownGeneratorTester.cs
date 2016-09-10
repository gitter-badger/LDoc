using LCore.LUnit;
using Xunit;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.LDoc.Markdown;
using Test_LDoc;
using Xunit.Abstractions;

namespace LDoc_Tests.LCore.LDoc.Markdown
    {
    /*
    Covering class: LCore.LDoc.Markdown.MarkdownGenerator
    */

    public partial class MarkdownGeneratorTester : XUnitOutputTester, IDisposable
        {
        public MarkdownGeneratorTester([NotNull] ITestOutputHelper Output) : base(Output)
            {
            }

        public void Dispose()
            {
            }


        [Fact]
        public void PieChartGeneration()
            {
//          var Gen = new LDocSolutionMarkdownGenerator();
//       Gen.SaveCategoryPieChart(new[]
//               {
//               "Public Method",
//               "Public Method",
//               "Public Method",
//               "Public Method",
//               "Public Method",
//               "Private Method",
//               "Public Method",
//               "Public Virtual Method",
//               "Public Virtual Method",
//               "Public Virtual Method",
//               "Public Virtual Method",
//               "Public Abstract Method",
//               "Public Abstract Method",
//               "Public Abstract Method",
//               "Public Override Method"
//               },
//           s => s,
//           @"C:\Users\Ben\Desktop\test.png");
            }

        [Fact]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(SolutionMarkdownGenerator) + "." + nameof(SolutionMarkdownGenerator.Generate) + "(Boolean)")]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(SolutionMarkdownGenerator) + "." + nameof(SolutionMarkdownGenerator.GetAllMarkdown) + "() => List<GitHubMarkdown>")]
        public void Generate()
            {
            var Gen = new LDocSolutionMarkdownGenerator();

            Gen.Generate(WriteToDisk: true);

            // List<GeneratedDocument> Markdown = Gen.GetAllMarkdown();
            // List<string> Paths = Markdown.Convert(MD => MD?.FullPath);
            // Paths.Each(this._Output.WriteLine);
            }
        }
    }