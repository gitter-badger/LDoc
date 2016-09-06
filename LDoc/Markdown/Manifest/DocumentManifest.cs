using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;

namespace LCore.LDoc.Markdown.Manifest
    {
    /// <summary>
    /// Stores information about a generated markdown document
    /// </summary>
    [Serializable]
    public class DocumentManifest
        {
        /// <summary>
        /// The fully qualified member name
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// The fully qualified URL to the member documentation
        /// </summary>
        public string FullUrl_Documentation { get; set; }

        /// <summary>
        /// Create a new <see cref="DocumentManifest"/>. 
        /// This constructor is used for JSON deserialization.
        /// </summary>
        public DocumentManifest()
            {

            }

        /// <summary>
        /// Create a <see cref="DocumentManifest"/> from a <see cref="GeneratedDocument"/>.
        /// </summary>
        public DocumentManifest(GeneratedDocument Doc)
            {
            this.FullUrl_Documentation = Doc.GetLiveUrl();

            if (Doc is MarkdownDocument_Member)
                {
                var Member = ((MarkdownDocument_Member)Doc).Member;
                if (Member is MethodInfo)
                    {
                    var Method = (MethodInfo)Member;
                    this.MemberName = Method.ToInvocationSignature();
                    }
                else
                    {
                    this.MemberName = ((MarkdownDocument_Member)Doc).Member.FullyQualifiedName();
                    }
                }

            if (Doc is MarkdownDocument_Type)
                this.MemberName = (((MarkdownDocument_Type)Doc).TypeMeta.Member as Type).FullyQualifiedName();

            if (Doc is MarkdownDocument_Assembly)
                this.MemberName = ((MarkdownDocument_Assembly)Doc).Assembly.GetName().Name;
            }
        }
    }