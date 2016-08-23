using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.LDoc;
using LCore.LDoc.Markdown;

namespace Test_LDoc
    {
    public class LDocMarkdownGenerator : MarkdownGenerator
        {
        protected override Assembly[] DocumentAssemblies => new[] {Assembly.GetAssembly(typeof(LDoc))};
        
        }
    }