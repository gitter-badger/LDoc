using LCore.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using JetBrains.Annotations;
using LCore.Tools;

// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Helper class for generating GitHub markdown documents.
    /// </summary>
    public class GitHubMarkdown
        {
        /// <summary>
        /// The path relative to the root repository folder that this markdown file will be saved
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The title of the markdown file
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The generator that created this Markdown, if applicable.
        /// </summary>
        [CanBeNull]
        protected MarkdownGenerator Generator { get; }

        /// <summary>
        /// Create a new GitHumMarkdown document without specifying a file title or location
        /// </summary>
        public GitHubMarkdown() { }

        /// <summary>
        /// Create a new GitHumMarkdown document specifying a file title and location
        /// </summary>
        public GitHubMarkdown([CanBeNull]MarkdownGenerator Generator, [CanBeNull] string FilePath, [CanBeNull] string Title)
            {
            this.Generator = Generator;
            this.FilePath = FilePath ?? "";
            this.Title = Title ?? "";
            }

        /// <summary>
        /// List of all Markdown Lines added.
        /// </summary>
        protected List<string> MarkdownLines { get; } = new List<string>();

        /// <summary>
        /// Gets a list of all markdown lines.
        /// </summary>
        public List<string> GetMarkdownLines()
            {
            return this.MarkdownLines.List();
            }

        /// <summary>
        /// Add a blank line:
        /// </summary>
        public void BlankLine()
            {
            this.Line("");
            }

        /// <summary>
        /// Add a horizontal rule:
        /// 
        /// ---
        /// 
        /// 
        /// </summary>
        public void HorizontalRule()
            {
            this.Line("");
            this.Line("---");
            this.Line("");
            }

        /// <summary>
        /// Add a header line:
        /// 
        /// # Header
        /// ## Header
        /// ### Header
        /// #### Header
        /// ##### Header
        /// ###### Header
        /// 
        /// </summary>
        public void Header(string Line, int Size = 1)
            {
            if (Size < 1)
                Size = 1;
            if (Size > 6)
                Size = 6;

            this.Line("");
            this.Line($"{"#".Times(Size)} {Line}");
            }

        /// <summary>
        /// Add a header underlined:
        /// 
        /// Line
        /// ======
        /// 
        /// Line 
        /// ------
        /// </summary>
        public void HeaderUnderline(string Line, int Size = 1)
            {
            if (Size < 1)
                Size = 1;
            if (Size > 2)
                Size = 2;

            this.Line("");
            this.Line($"{Line}");
            this.Line(Size == 1
                ? "======"
                : "------");
            }

        /// <summary>
        /// Add an ordered list
        /// 
        /// 1. Line
        /// 2. Line
        /// 3. Line
        /// 
        /// </summary>
        public void OrderedList([CanBeNull] params string[] Lines)
            {
            Lines.Each((i, Line) => this.Line($"{i + 1}. {Line}"));
            }

        /// <summary>
        /// Add an ordered list with indentation
        /// 
        /// 1. Item
        /// 2. Item
        ///     1. Subitem
        ///     2. Subitem
        ///     3. Subitem
        /// 3. Item
        /// 
        /// </summary>
        public void OrderedList([CanBeNull] params Tuple<uint, string>[] DepthLine)
            {
            DepthLine.Each(Line => this.OrderedList((Set<uint, string>)Line));
            }

        /// <summary>
        /// Add an ordered list with indentation
        /// 
        /// 1. Item
        /// 2. Item
        ///     1. Subitem
        ///     2. Subitem
        ///     3. Subitem
        /// 3. Item
        /// 
        /// </summary>
        public void OrderedList([CanBeNull] params Set<uint, string>[] DepthLine)
            {
            uint CurrentNumber = 0;
            uint? LastLevel = null;

            DepthLine.Each(Line =>
                {
                    if (LastLevel == null || Line.Obj1 != LastLevel)
                        CurrentNumber = 1;

                    this.Line($"{"  ".Times(Line.Obj1)}{CurrentNumber}{Line.Obj2}");

                    LastLevel = Line.Obj1;
                    CurrentNumber++;
                });
            }

        /// <summary>
        /// Add an unordered list
        /// 
        /// - Line
        /// - Line
        /// - Line
        /// 
        /// </summary>
        public void UnorderedList([CanBeNull] params string[] Lines)
            {
            Lines.Each(Line => this.Line($"- {Line}"));
            }

        /// <summary>
        /// Add an unordered list with indentation
        /// 
        /// - Item
        /// - Item
        ///     - Subitem
        ///     - Subitem
        ///     - Subitem
        /// - Item
        /// 
        /// </summary>
        public void UnorderedList([CanBeNull] params Tuple<uint, string>[] DepthLine)
            {
            DepthLine.Each(Line => this.UnorderedList((Set<uint, string>)Line));
            }

        /// <summary>
        /// Add an unordered list with indentation
        /// 
        /// - Item
        /// - Item
        ///     - Subitem
        ///     - Subitem
        ///     - Subitem
        /// - Item
        /// 
        /// </summary>
        public void UnorderedList([CanBeNull] params Set<uint, string>[] DepthLine)
            {
            DepthLine.Each(Line => { this.Line($"{"  ".Times(Line.Obj1)}*{Line.Obj2}"); });
            }

        /// <summary>
        /// Add a number of lines of code, optionally include a Language for 
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Language"></param>
        public void Code([CanBeNull] string[] Lines = null, [CanBeNull] string Language = MarkdownGenerator.CSharpLanguage)
            {
            this.Line($"```{Language}");
            Lines.Each(this.Line);
            this.Line("```");
            }

        /// <summary>
        /// Add a table of data.
        /// By default, the first row will be used as the header row, and separator will be added
        /// 
        /// Header |  Header | Header
        /// -------------------------
        /// Data | Data | Data
        /// Data | Data | Data
        /// 
        /// //////////////////////////////////////////
        /// 
        /// Data | Data | Data
        /// Data | Data | Data
        /// Data | Data | Data
        /// 
        /// </summary>
        public void Table([CanBeNull] string[,] Rows, bool IncludeHeader = true, L.Align[] Alignment = null)
            {
            this.Table(Rows.ToNestedArrays(), IncludeHeader, Alignment);
            }

        /// <summary>
        /// Add a table of data.
        /// By default, the first row will be used as the header row, and separator will be added
        /// 
        /// Header |  Header | Header
        /// -------------------------
        /// Data | Data | Data
        /// Data | Data | Data
        /// 
        /// //////////////////////////////////////////
        /// 
        /// Data | Data | Data
        /// Data | Data | Data
        /// Data | Data | Data
        /// 
        /// </summary>
        public void Table([CanBeNull] IEnumerable<IEnumerable<string>> Rows, bool IncludeHeader = true, L.Align[] Alignment = null)
            {
            if (Rows == null)
                return;

            var Table = new List<string>();
            var Divider = new List<string>();

            Rows.Each((i, Row) =>
                {
                    var Cells = new List<string>();

                    Row.Each((j, Column) =>
                        {
                            Cells.Add(Column);

                            if (IncludeHeader && i == 0)
                                {
                                L.Align? Align = Alignment.GetAt(j);

                                if (Align == L.Align.Left)
                                    Divider.Add(":--- ");
                                else if (Align == L.Align.Right)
                                    Divider.Add(" ---:");
                                else if (Align == L.Align.Center)
                                    Divider.Add(":---:");
                                else
                                    Divider.Add(" --- ");
                                }
                        });

                    Table.Add(Cells.JoinLines(" | "));
                    if (IncludeHeader && i == 0)
                        Table.Add(Divider.JoinLines(" | "));
                });

            this.Line("");
            Table.Each(this.Line);
            this.Line("");
            }

        /// <summary>
        /// Adds a blockquoted series of <paramref name="Lines"/>
        /// </summary>
        public void BlockQuote([CanBeNull] params string[] Lines)
            {
            Lines.Each(Line => this.Line($"> {Line}"));
            }

        /// <summary>
        /// Add a number of <paramref name="Lines"/>
        /// </summary>
        public void Lines([CanBeNull] params string[] Lines)
            {
            Lines?.Each(this.Line);
            }

        /// <summary>
        /// Add a single <paramref name="Line"/>
        /// </summary>
        public void Line([CanBeNull] string Line)
            {
            if (Line != null)
                this.MarkdownLines.Add(Line);
            }



        /// <summary>
        /// Returns strikethrough line
        /// 
        /// ~Line~
        /// 
        /// </summary>
        public string Strikethrough([CanBeNull] string Text)
            {
            return !string.IsNullOrEmpty(Text)
                ? $"~~{Text}~~"
                : "";
            }

        /// <summary>
        /// Returns highlighted test
        /// 
        /// =Line=
        /// 
        /// </summary>
        public string Highlight([CanBeNull] string Text)
            {
            return !string.IsNullOrEmpty(Text)
                ? $"=={Text}=="
                : "";
            }

        /// <summary>
        /// Returns a link, all arguments are optional
        /// 
        /// (Url)
        /// [Text]
        /// [Text](Url)
        /// [Text](Url)"Reference Text"
        /// 
        /// </summary>
        public string Link([CanBeNull] string Url = "", [CanBeNull] string Text = "", [CanBeNull] string ReferenceText = "", bool TargetNewWindow = false, bool EscapeText = true)
            {
            if (EscapeText)
                Text = WebUtility.HtmlEncode(Text);

            if (TargetNewWindow)
                return $"<a href=\"{Url}\" alt=\"{ReferenceText}\" target=\"_blank\">{Text}</a>";

            if (!string.IsNullOrEmpty(Url))
                {
                Url = $"({Url})";
                if (!string.IsNullOrEmpty(ReferenceText))
                    ReferenceText = $"[{ReferenceText}]";
                }
            if (!string.IsNullOrEmpty(ReferenceText))
                ReferenceText = $" \"{ReferenceText}\"";

            return $"[{Text}]{Url}{ReferenceText}";
            }

        /// <summary>
        /// Returns an image link, optionally with Reference Text
        /// 
        /// !(Image Url)
        /// ![Reference Text](Image Url)
        /// 
        /// </summary>
        public string Image([CanBeNull] string Url, [CanBeNull] string ReferenceText = "", L.Align? Align = null)
            {
            return Align == null
                ? $"![{ReferenceText}]({Url} \"\")"
                : $"<img align=\"{Align.ToString().ToLower()}\" src=\"{Url}\">";
            }

        /// <summary>
        /// Returns a string formatted as inline code
        /// </summary>
        public string InlineCode([CanBeNull] string Code = "")
            {
            return Code == null
                ? ""
                : $"`{Code}`";
            }

        /// <summary>
        /// Returns a string formatted in italics
        /// 
        /// *Text*
        /// 
        /// </summary>
        public string Italic([CanBeNull] string Text = "")
            {
            return Text == null
                ? ""
                : $"*{Text}*";
            }

        /// <summary>
        /// Returns a string formatted as bold
        /// 
        /// **Text**
        /// 
        /// </summary>
        public string Bold([CanBeNull] string Text = "")
            {
            return Text == null
                ? ""
                : $"**{Text}**";
            }
        /*

                /// <summary>
                /// Formats a glyphicon for display in a markdown document
                /// </summary>
                public string Glyph(GlyphIcon Glyph)
                    {
                    return $"<span class=\"glyphicon glyphicon-{Glyph.ToString().ToLower().Trim('_').ReplaceAll("_", "-")}\"></span>";
                    }
        */

        /// <summary>
        /// Adds a Buckler badge, hosted on http://b.repl.ca/
        /// </summary>
        public string Badge(string Left, string Right, string HexColor)
            {
            return this.Image(
                    "http://b.repl.ca/v1/" +
                    $"{Left.UriEncode()}-{Right.UriEncode()}-{HexColor}.png",
                    $"{Left} {Right}");
            }

        /// <summary>
        /// Adds a Buckler badge, hosted on http://b.repl.ca/
        /// </summary>
        public string Badge(string Left, string Right, BadgeColor Color = BadgeColor.LightGrey)
            {
            return this.Badge(Left, Right, Color.ToString().ToLower());
            }

        /// <summary>
        /// Pre-defined Buckler badge colors (http://b.repl.ca/)
        /// </summary>
        public enum BadgeColor
            {
#pragma warning disable 1591
            BrightGreen, Green, YellowGreen, Yellow, Orange, Red, Blue, LightGrey, Grey
#pragma warning restore 1591
            }

        /// <summary>
        /// Retrieves the relative path from this markdown file to <paramref name="FullPath"/>
        /// </summary>
        public string GetRelativePath([CanBeNull]string FullPath)
            {
            if (FullPath == null)
                return "";

            if (string.IsNullOrEmpty(this.FilePath))
                return FullPath;


            var Uri1 = new Uri(FullPath);
            var Uri2 = new Uri(this.FilePath);

            var Out = Uri2.MakeRelativeUri(Uri1);

            return Out.ToString();
            }

        /// <summary>
        /// Returns an image link to a Gravatar avatar based on the MD5 of the supplied <paramref name="ID"/>
        /// </summary>
        public string Gravatar(string ID, int Size = 64)
            {
            string URL = "https://www.gravatar.com/avatar/";

            var MD5 = new MD5CryptoServiceProvider();


            byte[] b = MD5.ComputeHash(ID.ToByteArray());

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (byte T in b)
                {
                URL = $"{URL}{T.ToString("X2").ToLower()}";
                }

            URL += $"?s={Size}";
            URL += "&d;=identicon";
            URL += "&r;=PG;";

            return this.Image(URL, ID);
            }
        }
    }