using Xunit.Abstractions;
using LCore.LUnit;
using Xunit;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.LDoc.Markdown;
using Test_LDoc;

namespace LDoc_Tests.LCore.LDoc.Markdown
    {
    /*
    Covering class: LCore.LDoc.Markdown.MarkdownGenerator
    */
    public partial class MarkdownGeneratorTester : XUnitOutputTester, IDisposable
        {
        public MarkdownGeneratorTester([NotNull] ITestOutputHelper Output) : base(Output) { }

        public void Dispose() { }


        [Fact]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(MarkdownGenerator) + "." + nameof(MarkdownGenerator.MarkdownPath_Assembly) + "(Assembly) => String")]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(MarkdownGenerator) + "." + nameof(MarkdownGenerator.MarkdownPath_Type) + "(Type) => String")]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(MarkdownGenerator) + "." + nameof(MarkdownGenerator.MarkdownPath_Member) + "(MemberInfo) => String")]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(MarkdownGenerator) + "." + nameof(MarkdownGenerator.Generate) + "(Boolean)")]
        [Trait(Traits.TargetMember, nameof(LCore) + "." + nameof(global::LCore.LDoc) + "." + nameof(global::LCore.LDoc.Markdown) + "." + nameof(MarkdownGenerator) + "." + nameof(MarkdownGenerator.GetAllMarkdown) + "() => List<GitHubMarkdown>")]
        public void Generate()
            {
            var Gen = new LDocMarkdownGenerator();

            Gen.Generate(WriteToDisk: true);

            List<GitHubMarkdown> Markdown = Gen.GetAllMarkdown();

            List<string> Paths = Markdown.Convert(MD => MD?.FilePath);

            Paths.Each(this._Output.WriteLine);
            }


        }
    }
