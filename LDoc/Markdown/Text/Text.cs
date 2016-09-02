using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Structure to customize text used in MarkdownGenerator
    /// </summary>
    public class Text
        {
        /// <summary>
        /// Main readme title, default is "Home"
        /// </summary>
        public string MainReadme { get; set; } = "Home";

        /// <summary>
        /// Table of Contents readme title, default is "Table of Contents"
        /// </summary>
        public string TableOfContents { get; set; } = "Table of Contents";

        /// <summary>
        /// Coverage Summary readme title, default is "Coverage Summary"
        /// </summary>
        public string CoverageSummary { get; set; } = "Coverage Summary";

        /// <summary>
        /// Coverage summary file name, default is "CoverageSummary.md"
        /// </summary>
        public string CoverageSummaryFile { get; set; } = "CoverageSummary.md";

        /// <summary>
        /// Table of contents file name, default is "TableOfContents.md"
        /// </summary>
        public string TableOfContentsFile { get; set; } = "TableOfContents.md";

        /// <summary>
        /// Badge title for project framework
        /// </summary>
        public string Badge_Framework { get; set; } = "Framework";

        /// <summary>
        /// Badge title for object Type
        /// </summary>
        public string Badge_Type { get; set; } = "Type";

        /// <summary>
        /// Badge title for lines of code
        /// </summary>
        public string Badge_LinesOfCode { get; set; } = "Lines of Code";

        /// <summary>
        /// Badge title for to-dos
        /// </summary>
        public string Badge_TODOs { get; set; } = "TODOs";

        /// <summary>
        /// Badge title for bugs
        /// </summary>
        public string Badge_BUGs { get; set; } = "Bugs";

        /// <summary>
        /// Badge title for NotImplementedException
        /// </summary>
        public string Badge_NotImplemented { get; set; } = "Not Implemented";


        /// <summary>
        /// Badge title for member Documented
        /// </summary>
        public string Badge_Documented { get; set; } = "Documented";

        /// <summary>
        /// Badge title for member Assertions
        /// </summary>
        public string Badge_Assertions { get; set; } = "Assertions";

        /// <summary>
        /// Badge title for member UnitTested
        /// </summary>
        public string Badge_UnitTested { get; set; } = "Unit Tested";

        /// <summary>
        /// Badge title for member Covered
        /// </summary>
        public string Badge_Covered { get; set; } = "Covered";

        /// <summary>
        /// Badge title for member Source Code
        /// </summary>
        public string Badge_SourceCode { get; set; } = "Source Code";

        /// <summary>
        /// Badge title for member Source Code Available
        /// </summary>
        public string Badge_SourceCodeAvailable { get; set; } = "Available";

        /// <summary>
        /// Badge title for member Source Code Unavailable
        /// </summary>
        public string Badge_SourceCodeUnavailable { get; set; } = "Unavailable";

        /// <summary>
        /// Badge title for member AttributeTests
        /// </summary>
        public string Badge_AttributeTests { get; set; } = "Attribute Tests";

        /// <summary>
        /// Badge title for Assemblies Header
        /// </summary>
        public string Header_Assemblies { get; set; } = "Assemblies";

        /// <summary>
        /// Badge title for Installation Instructions Header
        /// </summary>
        public string Header_InstallationInstructions { get; set; } = "Installation Instructions";

        /// <summary>
        /// Header for related projects
        /// </summary>
        public string Header_RelatedProjects { get; set; } = "Related Projects";

        /// <summary>
        /// Header for method parameters
        /// </summary>
        public string Header_MethodParameters { get; set; } = "Parameters";

        /// <summary>
        /// Header for method returns
        /// </summary>
        public string Header_MethodReturns { get; set; } = "Returns";

        /// <summary>
        /// Header for method summary
        /// </summary>
        public string Header_Summary { get; set; } = "Summary";

        /// <summary>
        /// Header for method examples
        /// </summary>
        public string Header_MethodExamples { get; set; } = "Method Examples";

        /// <summary>
        /// Header for method permissions
        /// </summary>
        public string Header_MethodPermissions { get; set; } = "Method Permissions";

        /// <summary>
        /// Header for method exceptions
        /// </summary>
        public string Header_MethodExceptions { get; set; } = "Method Exceptions";

        /// <summary>
        /// Header for code lines
        /// </summary>
        public string Header_CodeLines { get; set; } = "Code Lines";

        /// <summary>
        /// Header for documentation
        /// </summary>
        public string Header_Documentation { get; set; } = "Documentation";

        /// <summary>
        /// Header for coverage
        /// </summary>
        public string Header_Coverage { get; set; } = "Coverage";


        /// <summary>
        /// Link text for view source
        /// </summary>
        public string LinkText_ViewSource { get; set; } = "View Source";

        /// <summary>
        /// Link text for home
        /// </summary>
        public string LinkText_Home { get; set; } = "Home";

        /// <summary>
        /// Link text for up
        /// </summary>
        public string LinkText_Up { get; set; } = "Up";

        /// <summary>
        /// Alt text for the logo
        /// </summary>
        public string AltText_Logo { get; set; } = "Logo";

        /// <summary>
        /// Table header text method parameter
        /// </summary>
        public string TableHeaderText_MethodParameter { get; set; } = "Parameter";

        /// <summary>
        /// Table header text optional
        /// </summary>
        public string TableHeaderText_Optional { get; set; } = "Optional";

        /// <summary>
        /// Table header text type
        /// </summary>
        public string TableHeaderText_Type { get; set; } = "Type";

        /// <summary>
        /// Table header text description
        /// </summary>
        public string TableHeaderText_Description { get; set; } = "Description";

        /// <summary>
        /// Table header statistics
        /// </summary>
        public string TableHeader_Statistics { get; set; } = "Statistics";

        /// <summary>
        /// Table header file
        /// </summary>
        public string TableHeaderText_File { get; set; } = "File";

        /// <summary>
        /// Table header line
        /// </summary>
        public string TableHeaderText_Line { get; set; } = "Line";


        /// <summary>
        /// The default file name for JSON manifest documents
        /// </summary>
        public string ManifestFile { get; set; } = "document-manifest.json";

        /// <summary>
        /// Table header for errors
        /// </summary>
        public string TableHeader_Errors { get; set; } = "Errors";
        }
    }