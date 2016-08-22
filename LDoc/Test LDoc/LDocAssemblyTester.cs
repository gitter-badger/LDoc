using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LCore.LDoc;
using LCore.LDoc.Markdown;
using LCore.LUnit;
using Xunit.Abstractions;

namespace Test_LDoc
    {
    public class LDocAssemblyTester : AssemblyTester
        {
        protected override Type AssemblyType => typeof(LDoc);

        protected override bool EnableCodeAutoGeneration => true;

        public LDocAssemblyTester([NotNull] ITestOutputHelper Output) : base(Output) { }
        }

    public class LDocMarkdownGenerator : MarkdownGenerator
        {
        protected override Assembly[] DocumentAssemblies => new[] {Assembly.GetAssembly(typeof(LDoc))};
        
        }
    }
