![](Content/LDoc-banner-small.png "")
[Up](docs/LDoc.md)

###### namespace LCore.LDoc.Markdown

###### public abstract class GeneratedDocument

### GeneratedDocument

 ![Type Abstract Class](http://b.repl.ca/v1/Type-Abstract%20Class-blue.png "") ![Documented 90%](http://b.repl.ca/v1/Documented-90%25-green.png "")

![Covered 0%](http://b.repl.ca/v1/Covered-0%25-red.png "")

[View Source](Markdown/Generators/GeneratedDocument.cs#L)

###### Summary

            Abstract class for generating markdown documents
            

<table>
<thead><tr><td><h4>Public Override Methods <strong>(6)</strong></h4></td>
<td></td>
<td><img src="http://b.repl.ca/v1/Total%20Code%20Lines-22-blue.png" alt="Total Code Lines 22" /></td>
<td><img src="http://b.repl.ca/v1/Total%20Documentation-83%25-green.png" alt="Total Documentation 83%" /></td>
<td><img src="http://b.repl.ca/v1/Total%20Coverage-0%25-red.png" alt="Total Coverage 0%" /></td></tr></thead>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Line.md" alt="">Line</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L42" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-4-blue.png" alt="Lines of Code 4" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.void.aspx" alt="">void</a> <a href="" alt="">Line</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Line);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Table-0.md" alt="">Table</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L50" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-3-blue.png" alt="Lines of Code 3" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-No-red.png" alt="Documented No" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.void.aspx" alt="">void</a> <a href="" alt="">Table</a>(<a href="https://msdn.microsoft.com/en-us/library/78dfe2yb.aspx" alt="" target="_blank">IEnumerable</a>&lt;<a href="https://msdn.microsoft.com/en-us/library/78dfe2yb.aspx" alt="" target="_blank">IEnumerable</a>&lt;<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a>&gt;&gt; Rows, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> IncludeHeader, <a href="https://github.com/CodeSingularity/LCore/blob/master/L/docs/Align.md" alt="">L.Align</a>[] Alignment, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> AsHtml, <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> TableWidth);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Header.md" alt="">Header</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L59" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-4-blue.png" alt="Lines of Code 4" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> <a href="" alt="">Header</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Line, <a href="https://msdn.microsoft.com/en-us/library/system.int32.aspx" alt="">Int32</a> Size, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> AsHtml);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_HeaderUnderline.md" alt="">HeaderUnderline</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L67" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-4-blue.png" alt="Lines of Code 4" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.void.aspx" alt="">void</a> <a href="" alt="">HeaderUnderline</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Line, <a href="https://msdn.microsoft.com/en-us/library/system.int32.aspx" alt="">Int32</a> Size);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Link.md" alt="">Link</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L75" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-3-blue.png" alt="Lines of Code 3" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> <a href="" alt="">Link</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Url, <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Text, <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> ReferenceText, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> TargetNewWindow, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> EscapeText, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> AsHtml);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Badge-0.md" alt="">Badge</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L84" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-4-blue.png" alt="Lines of Code 4" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public override <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> <a href="" alt="">Badge</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Left, <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> Right, <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> HexColor, <a href="https://msdn.microsoft.com/en-us/library/system.boolean.aspx" alt="">Boolean</a> AsHtml);</h6>
</td>
</tr>
<tr><td width="850px" colspan="5"></td></tr>
</table>


<table>
<thead><tr><td><h4>Public Methods <strong>(3)</strong></h4></td>
<td></td>
<td><img src="http://b.repl.ca/v1/Total%20Code%20Lines-33-blue.png" alt="Total Code Lines 33" /></td>
<td><img src="http://b.repl.ca/v1/Total%20Documentation-100%25-brightgreen.png" alt="Total Documentation 100%" /></td>
<td><img src="http://b.repl.ca/v1/Total%20Coverage-0%25-red.png" alt="Total Coverage 0%" /></td></tr></thead>
<tr><td><h4><strong><a href="docs/GeneratedDocument_GetRelativePath.md" alt="">GetRelativePath</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L94" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-16-blue.png" alt="Lines of Code 16" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> <a href="" alt="">GetRelativePath</a>(<a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> FullPath);</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_Generate.md" alt="">Generate</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L130" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-6-blue.png" alt="Lines of Code 6" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public <a href="https://msdn.microsoft.com/en-us/library/system.void.aspx" alt="">void</a> <a href="" alt="">Generate</a>();</h6>
</td>
</tr>
<tr><td><h4><strong><a href="docs/GeneratedDocument_GetLiveUrl.md" alt="">GetLiveUrl</a></strong></h4></td>
<td>   </td>
<td><a href="Markdown/Generators/GeneratedDocument.cs#L146" alt=""><img src="http://b.repl.ca/v1/Lines%20of%20Code-11-blue.png" alt="Lines of Code 11" /></a></td>
<td><img src="http://b.repl.ca/v1/Documented-Yes-brightgreen.png" alt="Documented Yes" /></td>
<td><img src="http://b.repl.ca/v1/Covered-No-red.png" alt="Covered No" /></td></tr>
<tr><td align="Left" colspan="5"><h6>public <a href="https://msdn.microsoft.com/en-us/library/system.string.aspx" alt="">String</a> <a href="" alt="">GetLiveUrl</a>();</h6>
</td>
</tr>
<tr><td width="850px" colspan="5"></td></tr>
</table>




---

Copyright 2016 &copy; [Home](../README.md) [Table of Contents](../TableOfContents.md)

This markdown was generated by [LDoc](https://github.com/CodeSingularity/LDoc), powered by [LUnit](https://github.com/CodeSingularity/LUnit), [LCore](https://github.com/CodeSingularity/LCore)
