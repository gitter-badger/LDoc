using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LCore.Extensions;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace LCore.LDoc.Markdown.Manifest
    {
    /// <summary>
    /// Stores manifest information for automatically inter-linking LDoc projects
    /// </summary>
    public class LDocTypeManifest
        {
        /// <summary>
        /// Member documents generated (member fully qualified name, full url to document)
        /// </summary>
        public List<DocumentManifest> MemberDocuments { get; set; } = new List<DocumentManifest>();

        /// <summary>
        /// Create a new manifest file
        /// </summary>
        public LDocTypeManifest()
            {
            }

        /// <summary>
        /// Create a manifest file from a group of <see cref="GeneratedDocument"/>
        /// </summary>
        public LDocTypeManifest(IEnumerable<GeneratedDocument> Docs)
            {
            this.MemberDocuments = Docs
                .Select(Doc => Doc is MarkdownDocument_Type)
                .Convert(Doc => new DocumentManifest(Doc));
            }

        /// <summary>
        /// Retrieves a document for a given <param name="Member"></param>
        /// </summary>
        [CanBeNull]
        public DocumentManifest GetDocument(MemberInfo Member)
            {
            return this.MemberDocuments.First(Doc =>
                {
                    if (Member is MethodInfo)
                        return Doc.MemberName == ((MethodInfo)Member).ToInvocationSignature();
                    return Doc.MemberName == Member.FullyQualifiedName();
                });
            }

        /// <summary>
        /// Creates a JSON string for the manifest
        /// </summary>
        public string CreateManifestJSON()
            {
            return JsonConvert.SerializeObject(this);
            }

        /// <summary>
        /// Creates a <see cref="LDocTypeManifest"/> from a JSON <see cref="string"/>
        /// </summary>
        public static LDocTypeManifest FromJSON(string Data)
            {
            return JsonConvert.DeserializeObject<LDocTypeManifest>(Data);
            }

        /// <summary>
        /// Merges an existing <see cref="LDocTypeManifest"/> with a current document structure.
        /// This should only add changes since last generation.
        /// </summary>
        public void CaptureHistory(IEnumerable<GeneratedDocument> Docs)
            {
            // TODO merge local data with existing
            }
        }
    }