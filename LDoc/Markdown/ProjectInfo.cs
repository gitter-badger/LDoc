using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Naming;

namespace LCore.LDoc.Markdown
    {
    public class ProjectInfo : INamed
        {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
        }
    }