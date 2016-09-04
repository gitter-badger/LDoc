using System;
using System.Collections;
using System.Collections.Generic;
using LCore.Extensions;
using LCore.LUnit;

// ReSharper disable SuggestBaseTypeForParameter

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Generates markdown for the tag summary file of a project.
    /// </summary>
    public class MarkdownDocument_TagSummary : GeneratedDocument
        {
        /// <summary>
        /// 
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// All lines found in the project for the current <see cref="TagName"/>
        /// </summary>
        public List<CodeLineInfo> TagLines { get; }

        /// <summary>
        /// Create a new root Markdown file.
        /// </summary>
        public MarkdownDocument_TagSummary(SolutionMarkdownGenerator Generator, string Title, string TagName)
            : base(Generator, Title)
            {
            this.TagName = TagName;
            this.TagLines = new List<CodeLineInfo>();

            this.Generator.Markdown_Type.Each(Type =>
                {
                    var Comments = Type.Key.GatherCodeCoverageMetaData(this.Generator.CustomCommentTags);

                    if (this.TagName.ToLower() == "todo")
                        this.TagLines.AddRange(Comments?.CommentTODO);
                    else if (this.TagName.ToLower() == "bug")
                        this.TagLines.AddRange(Comments?.CommentBUG);
                    else if (this.TagName.ToLower() == "throw new NotImplementedException")
                        this.TagLines.AddRange(Comments?.NotImplemented);
                    else
                        {
                        List<CodeLineInfo> Tags = Comments?.CommentTags.SafeGet(this.TagName);
                        if (Tags != null)
                            this.TagLines.AddRange(Tags);
                        }
                });
            }

        /// <inheritdoc />
        protected override string FileName => $"TagSummary_{this.TagName.CleanFileName()}.md";

        /// <inheritdoc />
        protected override string FilePath => this.Generator.GeneratedMarkdownRoot;

        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.ProjectMarkdownDocuments++;


            this.Line(this.Header(this.Title, Size: 3));
            this.Line(this.Header($"Total ({this.TagLines.Count})", Size: 4));

            Dictionary<string, List<CodeLineInfo>> FileTags = this.TagLines.Group(Line => Line.FilePath);

            FileTags.Each(File =>
                {
                    var Table = new List<List<string>>();

                    string Path = File.Value.First()?.FilePath;
                    Table.Add(new List<string>
                    {
                    "Line",
                    $"{this.Link($"{this.GetRelativePath(Path)}", $"{Path.AfterLast("\\")}")} ({File.Value.Count})"
                    //this.Generator.Language.TableHeaderText_Line
                    });

                    File.Value.Each(Tag =>
                        {
                            Table.Add(new List<string>
                            {
                        this.Link($"{this.GetRelativePath(Tag.FilePath)}#L{Tag.LineNumber}", $"{Tag.LineNumber}"),
                        Tag.LineText
                            });
                        });

                    this.Table(Table);
                });
            }
        }
    }