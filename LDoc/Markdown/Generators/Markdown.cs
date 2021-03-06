using LCore.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using JetBrains.Annotations;
using LCore.Tools;

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace LCore.LDoc.Markdown
    {
    /// <summary>
    /// Helper class for generating GitHub markdown documents.
    /// </summary>
    public class Markdown
        {
        // TODO find a service to provide glyphs


        /// <summary>
        /// The title of the markdown file
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Change this value to control rendering. 
        /// This is useful for rendering large blocks or whole tables as Html.
        /// </summary>
        public bool ForceHtml { get; set; }


        /// <summary>
        /// List of all Markdown Lines added.
        /// </summary>
        protected List<string> MarkdownLines { get; } = new List<string>();


        /// <summary>
        /// Create a new GitHumMarkdown document without specifying a file title or location
        /// </summary>
        public Markdown()
            {
            }

        /// <summary>
        /// Create a new GitHumMarkdown document specifying a file title and location
        /// </summary>
        public Markdown([CanBeNull] string Title)
            {
            this.Title = Title ?? "";
            }


        #region Lines

        /// <summary>
        /// Add a single <paramref name="Line"/>
        /// </summary>
        public virtual void Line([CanBeNull] string Line)
            {
            if (Line != null)
                this.MarkdownLines.Add(Line);
            }

        /// <summary>
        /// Add a number of <paramref name="Lines"/>
        /// </summary>
        public virtual void Lines([CanBeNull] params string[] Lines)
            {
            Lines?.Each(this.Line);
            }

        /// <summary>
        /// Add a blank line:
        /// </summary>
        public virtual void BlankLine(bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            this.Line(AsHtml ? "<br/> " : "");
            }

        /// <summary>
        /// Gets a list of all markdown lines.
        /// </summary>
        public List<string> GetMarkdownLines()
            {
            return this.MarkdownLines.List();
            }

        /// <summary>
        /// Clears the markdown document lines
        /// </summary>
        public void Clear()
            {
            this.MarkdownLines.Clear();
            this.MarkdownLines.TrimExcess();
            }
        #endregion

        #region Headers

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
        public virtual string Header(string Line, int Size = 1, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (Size < 1)
                Size = 1;
            if (Size > 6)
                Size = 6;

            return AsHtml
                ? $"<h{Size}>{Line}</h{Size}>"
                : $"\r\n{"#".Times(Size)} {Line}";
            }

        /// <summary>
        /// Returns a header line, extracting the <paramref name="Anchor"/> text into a variable
        /// </summary>
        public virtual string HeaderAnchor(string Line, [CanBeNull]out string Anchor, int Size = 1, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (Size < 1)
                Size = 1;
            if (Size > 6)
                Size = 6;

            Anchor = Line.ToUrlSlug();

            return AsHtml
                ? $"<h{Size}>{Line}</h{Size}>"
                : $"\r\n{"#".Times(Size)} {Line}";
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
        public virtual void HeaderUnderline(string Line, int Size = 1, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (Size < 1)
                Size = 1;
            if (Size > 2)
                Size = 2;

            this.Line("");
            this.Line(AsHtml ? $"<h{Size}>{Line}</h{Size}>" : $"{Line}");
            this.Line(Size == 1
                ? (AsHtml ? "<hr/>" : "======")
                : (AsHtml ? "<hr/>" : "------"));
            }

        /// <summary>
        /// Add a horizontal rule:
        /// 
        /// ---
        /// 
        /// 
        /// </summary>
        public virtual void HorizontalRule(bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            this.Line("");
            this.Line(AsHtml ? "<hr/> " : "---");
            this.Line("");
            }

        #endregion

        #region Text Formatting

        /// <summary>
        /// Adds a blockquoted series of <paramref name="Lines"/>
        /// </summary>
        public virtual void BlockQuote([CanBeNull] string[] Lines, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (AsHtml)
                this.Line("<blockquote>");

            Lines.Each(Line => this.Line(AsHtml ? Line : $"> {Line}"));

            if (AsHtml)
                this.Line("</blockquote>");
            }

        /// <summary>
        /// Returns a string formatted in italics
        /// 
        /// *Text*
        /// 
        /// </summary>
        public virtual string Italic([CanBeNull] string Text = "", bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (AsHtml)
                return $"<emphasis>{Text}</emphasis>";

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
        public virtual string Bold([CanBeNull] string Text = "", bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (AsHtml)
                return $"<strong>{Text}</strong>";

            return Text == null
                ? ""
                : $"**{Text}**";
            }

        /// <summary>
        /// Returns strikethrough line
        /// 
        /// ~Line~
        /// 
        /// </summary>
        public virtual string Strikethrough([CanBeNull] string Text, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            return string.IsNullOrEmpty(Text)
                ? ""
                : (AsHtml ? $"<strikethrough>{Text}</strikethrough>" : $"~~{Text}~~");
            }

        #endregion

        #region Image
        /// <summary>
        /// Returns an image link, optionally with Reference Text
        /// 
        /// !(Image Url)
        /// ![Reference Text](Image Url)
        /// 
        /// </summary>
        public virtual string Image([CanBeNull] string Url, [CanBeNull] string ReferenceText = "",
            L.Align? Align = null, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (AsHtml || Align != null)
                return Align == null
                    ? $"<img src=\"{Url}\" alt=\"{ReferenceText}\" />"
                    : $"<img align=\"{Align.ToString().ToLower()}\" src=\"{Url}\" alt=\"{ReferenceText}\" />";

            return $"![{ReferenceText}]({Url} \"\")";
            }
        #endregion

        #region Link


        /// <summary>
        /// Returns a link, all arguments are optional
        /// 
        /// (Url)
        /// [Text]
        /// [Text](Url)
        /// [Text](Url)"Reference Text"
        /// 
        /// </summary>
        public virtual string Link([CanBeNull] string Url = "", [CanBeNull] string Text = "",
            [CanBeNull] string ReferenceText = "", bool TargetNewWindow = false,
            bool EscapeText = true, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (EscapeText)
                Text = WebUtility.HtmlEncode(Text);

            if (TargetNewWindow)
                return $"<a href=\"{Url}\" alt=\"{ReferenceText}\" target=\"_blank\">{Text}</a>";

            if (AsHtml)
                return $"<a href=\"{Url}\" alt=\"{ReferenceText}\">{Text}</a>";

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



        #endregion

        #region Table

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
        public virtual void Table([CanBeNull] string[,] Rows, bool IncludeHeader = true,
            L.Align[] Alignment = null, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            this.Table(Rows.ToNestedArrays(), IncludeHeader, Alignment, AsHtml);
            }

        /// <summary>
        /// Add a table of data.
        /// 
        /// Header |  Header | Header
        /// --- | --- | ---
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
        /// <param name="Rows"></param>
        /// <param name="IncludeHeader">By default, the first row will be used as the header row, and separator will be added.</param>
        /// <param name="Alignment">Optionally, set alignment for each cell.</param>
        /// <param name="AsHtml">Optionally, render the table as Html.</param>
        /// <param name="TableWidth">Optionally, set the Html table CSS style.</param>
        public virtual void Table([CanBeNull] IEnumerable<IEnumerable<string>> Rows, bool IncludeHeader = true,
            L.Align[] Alignment = null, bool AsHtml = false, string TableWidth = "")
            {
            AsHtml = AsHtml || this.ForceHtml;

            if (Rows == null)
                return;

            var Table = new List<string>();
            var Divider = new List<string>();

            uint Cols = Rows.GetAt(Index: 0)?.Count() ?? 0;

            Rows.Each((i, Row) =>
            {
                var Cells = new List<string>();

                Row.Each((j, Column) =>
                {
                    Cells.Add(Column);

                    if (IncludeHeader && i == 0)
                        {
                        if (!AsHtml)
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
                        }
                });

                if (AsHtml)
                    {
                    if (Cells.Count < Cols)
                        {
                        string TableRow = "";
                        for (int j = 0; j < Cells.Count; j++)
                            {
                            L.Align? Align = Alignment.GetAt(j);
                            string CellAlignment = "";

                            // TODO test html cell alignment

                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                            if (Align != null)
                                CellAlignment = $" align=\"{Align}\"";

                            if (j == Cells.Count - 1)
                                TableRow += $"<td{CellAlignment} colspan=\"{Cols - j}\">{Cells[j]}</td>\r\n";
                            else
                                TableRow += $"<td{CellAlignment}>{Cells[j]}</td>\r\n";
                            }
                        Table.Add(TableRow);
                        }
                    else
                        Table.Add(Cells.Collect(Cell => $"<td>{Cell}</td>").JoinLines());
                    }
                else
                    {
                    Table.Add(Cells.JoinLines(" | "));
                    if (IncludeHeader && i == 0)
                        Table.Add(Divider.JoinLines(" | "));
                    }
            });

            this.Line("");

            if (AsHtml)
                {
                this.Line("<table>");

                Table.Each((i, Row) =>
                    this.Line(i == 0 && IncludeHeader
                        ? $"<thead><tr>{Row}</tr></thead>"
                        : $"<tr>{Row}</tr>"));

                if (!string.IsNullOrEmpty(TableWidth))
                    this.Line($"<tr><td width=\"{TableWidth}\" colspan=\"{Table[index: 0].Count("<td>")}\"></td></tr>");
                this.Line("</table>");
                }
            else
                Table.Each(this.Line);

            this.Line("");
            }


        #endregion

        #region Lists

        /// <summary>
        /// Add an ordered list
        /// 
        /// 1. Line
        /// 2. Line
        /// 3. Line
        /// 
        /// </summary>
        public virtual void OrderedList([CanBeNull] params string[] Lines)
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
        public virtual void OrderedList([CanBeNull] params Tuple<uint, string>[] DepthLine)
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
        public virtual void OrderedList([CanBeNull] params Set<uint, string>[] DepthLine)
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
        public virtual void UnorderedList([CanBeNull] params string[] Lines)
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
        public virtual void UnorderedList([CanBeNull] params Tuple<uint, string>[] DepthLine)
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
        public virtual void UnorderedList([CanBeNull] params Set<uint, string>[] DepthLine)
            {
            DepthLine.Each(Line => { this.Line($"{"  ".Times(Line.Obj1)}*{Line.Obj2}"); });
            }


        #endregion

        #region Badges

        /// <summary>
        /// Adds a Buckler badge, hosted on http://b.repl.ca/
        /// </summary>
        public virtual string Badge(string Left, string Right, string HexColor, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            return this.Image(
                "http://b.repl.ca/v1/" +
                $"{Left.UriEncode()}-{Right.UriEncode()}-{HexColor}.png",
                $"{Left} {Right}", Align: null, AsHtml: AsHtml);
            }

        /// <summary>
        /// Adds a Buckler badge, hosted on http://b.repl.ca/
        /// </summary>
        public virtual string Badge(string Left, string Right, BadgeColor Color = BadgeColor.LightGrey, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            return this.Badge(Left, Right, Color.ToString().ToLower(), AsHtml);
            }


        #endregion

        #region Code

        /// <summary>
        /// Add a number of lines of code, optionally include a Language for 
        /// </summary>
        public virtual void Code([CanBeNull] string[] Lines = null, [CanBeNull] string Language = SolutionMarkdownGenerator.CSharpLanguage, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            this.Line(AsHtml ? $"<pre><code language=\"{Language}\">" : $"```{Language}");
            Lines.Each(this.Line);
            this.Line(AsHtml ? "</code></pre>" : "```");
            }

        /// <summary>
        /// Returns a string formatted as inline code
        /// </summary>
        public virtual string InlineCode([CanBeNull] string Code = "", bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

            return Code == null
                ? ""
                : AsHtml ? $"<code>{Code}</code>" : $"`{Code}`";
            }
        #endregion

        /// <summary>
        /// Returns an image link to a Gravatar avatar based on the MD5 of the supplied <paramref name="ID"/>
        /// </summary>
        /// TODO test Gravatar 
        public virtual string Gravatar(string ID, int Size = 64, bool AsHtml = false)
            {
            AsHtml = AsHtml || this.ForceHtml;

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

            return this.Image(URL, ID, Align: null, AsHtml: AsHtml);
            }
        }
    }