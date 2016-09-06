using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;

namespace LCore.LDoc.Markdown.Manifest
    {
    /// <summary>
    /// Stores historical member information
    /// </summary>
    public class MemberHistory
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
        /// The current status of the member
        /// </summary>
        public MemberHistoryRecord Current { get; set; }

        /// <summary>
        /// History of member information
        /// </summary>
        public List<MemberHistoryRecord> History { get; set; }

        /// <summary>
        /// Create a new <see cref="DocumentManifest"/>. 
        /// This constructor is used for JSON deserialization.
        /// </summary>
        public MemberHistory()
            {
            this.History = this.History ?? new List<MemberHistoryRecord>();
            }

        /// <summary>
        /// Create a <see cref="DocumentManifest"/> from a <see cref="GeneratedDocument"/>.
        /// </summary>
        public MemberHistory(GeneratedDocument Doc)
            {
            this.History = this.History ?? new List<MemberHistoryRecord>();

            this.FullUrl_Documentation = Doc.GetLiveUrl();

            MemberInfo Member = null;

            if (Doc is MarkdownDocument_Member)
                {
                Member = ((MarkdownDocument_Member)Doc).Member;
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
                {
                Member = ((MarkdownDocument_Type)Doc).Type;

                this.MemberName = (((MarkdownDocument_Type)Doc).TypeMeta.Member as Type).FullyQualifiedName();
                }


            var LastHistory = this.History.Last();
            MemberHistoryRecord NewHistory = null;

            if (Doc is MarkdownDocument_Type)
                {
                var Type = ((MarkdownDocument_Type)Doc).Type;
                this.Current = new TypeHistoryRecord(Type);

                NewHistory = LastHistory == null
                    ? this.Current
                    : new TypeHistoryRecord((TypeHistoryRecord)LastHistory, Type);
                }
            if (Doc is MarkdownDocument_Member)
                {
                this.Current = new MemberHistoryRecord(((MarkdownDocument_Member)Doc).Member);

                NewHistory = LastHistory == null
                    ? this.Current
                    : new MemberHistoryRecord(LastHistory, Member);
                }

            if (NewHistory != null && NewHistory.IsChanged)
                this.History.Add(NewHistory);
            }

        /// <summary>
        /// Updates the record with current data, adding a history item and updating the current record 
        /// if there are any changes.
        /// </summary>
        public void UpdateCurrentData()
            {
            if (this.MemberName == null)
                return;

            // TODO ensure FindMembers properly handles method invocation signatures
            var Member = L.Ref.FindMembers(this.MemberName).First()
                ?? L.Ref.FindType(this.MemberName);

            var Difference = new MemberHistoryRecord(this.Current, Member);

            if (Difference.IsChanged)
                {
                this.Current = new MemberHistoryRecord(Member);

                Difference.DataTime = $"{DateTime.UtcNow}";
                this.History.Add(Difference);
                }
            }
        }
    }