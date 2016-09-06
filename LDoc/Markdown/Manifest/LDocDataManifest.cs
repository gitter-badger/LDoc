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
    public class LDocDataManifest
        {
        /// <summary>
        /// Member documents generated (member fully qualified name, full url to document)
        /// </summary>
        public List<MemberHistory> MemberDocuments { get; set; }


        /// <summary>
        /// Create a new data manifest file 
        /// </summary>
        public LDocDataManifest()
            {
            this.MemberDocuments = new List<MemberHistory>();
            }

        /// <summary>
        /// Create a manifest file from a group of <see cref="GeneratedDocument"/>
        /// </summary>
        public LDocDataManifest(IEnumerable<GeneratedDocument> Docs)
            {
            Docs.Select(Doc => !(Doc is MarkdownDocument_Assembly))
                .Each(this.LoadDocument);
            }

        private void LoadDocument(GeneratedDocument Doc)
            {
            this.MemberDocuments = this.MemberDocuments ?? new List<MemberHistory>();

            var NewDoc = new MemberHistory(Doc);
            var ExistingDoc = this.MemberDocuments.First(Document => Document.MemberName == NewDoc.MemberName);
            if (ExistingDoc != null)
                {
                ExistingDoc.UpdateCurrentData();
                }
            else
                {
                this.MemberDocuments.Add(NewDoc);
                }
            }


        /// <summary>
        /// Retrieves a document for a given <param name="Member"></param>
        /// </summary>
        [CanBeNull]
        public MemberHistory GetDocument(MemberInfo Member)
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
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

        /// <summary>
        /// Creates a <see cref="LDocTypeManifest"/> from a JSON <see cref="string"/>
        /// </summary>
        public static LDocDataManifest FromJSON(string Data)
            {
            return JsonConvert.DeserializeObject<LDocDataManifest>(Data);
            }


        /// <summary>
        /// Merges an existing <see cref="LDocDataManifest"/> with a current document structure.
        /// This should only add changes since last generation.
        /// </summary>
        public void CaptureHistory(IEnumerable<GeneratedDocument> Docs)
            {
            Docs.Select(Doc => !(Doc is MarkdownDocument_Assembly))
                .Each(this.LoadDocument);

            // TODO map deleted
            // TODO map renamed
            }
        }
    }