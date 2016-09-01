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
        /// Create a new root Markdown file.
        /// </summary>
        public MarkdownDocument_TagSummary(SolutionMarkdownGenerator Generator, string FilePath, string Title, string TagName)
            : base(Generator, FilePath, Title)
            {
            this.TagName = TagName;
            }

        /// <summary>
        /// Generate the document.
        /// </summary>
        protected override void GenerateDocument()
            {
            this.Generator.Stats.ProjectMarkdownDocuments++;

            this.Generator.WriteHeader(this);

            var Lines = new List<CodeLineInfo>();
            this.Generator.Markdown_Type.Each(Type =>
                {
                    var Comments = Type.Key.GatherCodeCoverageMetaData(this.Generator.CustomCommentTags);

                    if (this.TagName.ToLower() == "todo")
                        Lines.AddRange(Comments?.CommentTODO);
                    else if (this.TagName.ToLower() == "bug")
                        Lines.AddRange(Comments?.CommentBUG);
                    else if (this.TagName.ToLower() == "throw new NotImplementedException")
                        Lines.AddRange(Comments?.NotImplemented);
                    else
                        {
                        List<CodeLineInfo> Tags = Comments?.CommentTags.SafeGet(this.TagName);
                        if (Tags != null)
                            Lines.AddRange(Tags);
                        }

                    Dictionary<string, List<CodeLineInfo>> FileTags = Lines.Group(Line => Line.FilePath);

                    FileTags.Each(File =>
                        {
                            var Table = new List<List<string>>();

                            string Path = File.Value.First()?.FilePath;
                            Table.Add(new List<string>
                                {
                                $"{this.Link(this.GetRelativePath(Path), $"{this.Generator.Language.TableHeaderText_File}")} ({File.Value.Count})",
                                $"{this.Link($"{this.GetRelativePath(Path)}", Path.AfterLast("\\"))}"
                                //this.Generator.Language.TableHeaderText_Line
                                });

                            File.Value.Each(Tag =>
                                {
                                    Table.Add(new List<string>
                                        {
                                            this.Link($"{this.GetRelativePath(Tag.FilePath)}#L{Tag.LineNumber}", $"Line {Tag.LineNumber}"),
                                            Tag.LineText
                                        });
                                });

                            this.Table(Table);
                        });

                });
            this.Generator.WriteFooter(this);
            }
        }
    }