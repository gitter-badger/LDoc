using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LCore.Extensions;
using LCore.LUnit;
// ReSharper disable UnusedMember.Global

namespace LCore.LDoc.Markdown.Manifest
    {
    /// <summary>
    /// Individual Member record for a point in time
    /// </summary>
    public class MemberHistoryRecord
        {
        /// <summary>
        /// Determines if there has been a change since the last record, if applicable.
        /// </summary>
        [NonSerialized]
        public bool IsChanged;

        /// <summary>
        /// The point in time the data was gathered
        /// </summary>
        public string DataTime { get; set; } = $"{DateTime.UtcNow}";

        /// <summary>
        /// The type of member
        /// </summary>
        public string MemberType { get; set; }

        /// <summary>
        /// Strongly-typed enum of the type of member
        /// </summary>
        protected MemberType? MemberTypeEnum
            {
            get { return this.MemberType.ParseEnum<MemberType>(); }
            set { this.MemberType = value.ToString(); }
            }

        /// <summary>
        /// Whether or not the member is documented
        /// </summary>
        public bool? Documented { get; set; }

        /// <summary>
        /// Whether or not the member is covered, if it is a method
        /// </summary>
        public bool? Covered { get; set; }



        /// <summary>
        /// Total line count for the tracked member
        /// </summary>
        public uint? LineCount { get; set; }

        /// <summary>
        /// Create a <see cref="MemberHistoryRecord"/>
        /// </summary>
        public MemberHistoryRecord()
            {
            }

        /// <summary>
        /// Create a <see cref="MemberHistoryRecord"/> from a <see cref="MemberInfo"/>.
        /// This creates an initial record, will cause <see cref="IsChanged"/> to be true.
        /// </summary>
        public MemberHistoryRecord(MemberInfo Member)
            {
            this.IsChanged = true;

            this.LoadMember(Member);
            }

        private void LoadMember(MemberInfo Member)
            {
            var Meta = Member.GatherCodeCoverageMetaData(new string[] { });

            if (Meta != null)
                {
                this.LineCount = Meta.CodeLineCount ?? 0u;

                this.MemberTypeEnum = Meta.Details?.Type;
                this.Documented = Meta.Comments != null;
                this.Covered = Meta.Coverage?.IsCovered;
                }
            }

        /// <summary>
        /// Create <see cref="MemberHistoryRecord"/> from the last data point: <paramref name="LastCurrent"/>
        /// </summary>
        public MemberHistoryRecord(MemberHistoryRecord LastCurrent, MemberInfo Member)
            {
            var Meta = Member.GatherCodeCoverageMetaData(new string[] { });

            this.DataTime = $"{DateTime.UtcNow}";

            if (Meta != null)
                {
                if (LastCurrent.LineCount != (Meta.CodeLineCount ?? 0u))
                    {
                    this.IsChanged = true;
                    this.LineCount = Meta.CodeLineCount ?? 0u;
                    }
                if (LastCurrent.MemberTypeEnum != null && LastCurrent.MemberTypeEnum != Meta.Details.Type)
                    {
                    this.IsChanged = true;
                    this.MemberTypeEnum = Meta.Details.Type;
                    }
                if (LastCurrent.Documented != null && LastCurrent.Documented != (Meta.Comments != null))
                    {
                    this.IsChanged = true;
                    this.Documented = Meta.Comments != null;
                    }
                if (LastCurrent.Covered != null && LastCurrent.Covered != Meta.Coverage.IsCovered)
                    {
                    this.IsChanged = true;
                    this.Covered = Meta.Coverage.IsCovered;
                    }
                }
            }
        }
    }