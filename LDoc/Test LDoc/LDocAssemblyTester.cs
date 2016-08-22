﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LCore.LUnit;
using Xunit.Abstractions;

namespace Test_LDoc
    {
    public class LDocAssemblyTester : AssemblyTester
        {
        protected override Type AssemblyType => typeof(LDoc.LDoc);

        protected override bool EnableCodeAutoGeneration => true;

        public LDocAssemblyTester([NotNull] ITestOutputHelper Output) : base(Output) { }
        }
    }
