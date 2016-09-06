using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LCore.Extensions;
using LCore.LUnit;

namespace LCore.LDoc.Markdown.Manifest
    {
    /// <summary>
    /// Individual Type record for a point in time
    /// </summary>
    public class TypeHistoryRecord : MemberHistoryRecord
        {
        /// <summary>
        /// The type being tracked
        /// </summary>
        protected Type Type { get; set; }

        /// <summary>
        /// The base type of <see cref="Type"/>, if applicable.
        /// </summary>
        [CanBeNull]
        public string BaseType { get; set; }

        /// <summary>
        /// Interfaces <see cref="Type"/> defines.
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// The total number of members beneath the <see cref="Type"/>
        /// </summary>
        public uint Members { get; set; }

        // TODO track comments and tags

        /// <summary>
        /// Create a <see cref="TypeHistoryRecord"/> from a <see cref="Type"/>.
        /// This creates an initial record, will cause <see cref="MemberHistoryRecord.IsChanged"/> to be true.
        /// </summary>
        public TypeHistoryRecord(Type Type) : base(Type)
            {
            this.Type = Type;
            this.IsChanged = true;

            this.LoadType(Type);
            }

        /// <summary>
        /// Create <see cref="TypeHistoryRecord"/> from the last data point: <paramref name="LastCurrent"/>
        /// </summary>
        // ReSharper disable SuggestBaseTypeForParameter
        public TypeHistoryRecord(TypeHistoryRecord LastCurrent, Type Type) : base(LastCurrent, Type)
        // ReSharper restore SuggestBaseTypeForParameter
            {
            var Meta = Type.GatherCodeCoverageMetaData(new string[] { });

            if (Meta != null)
                {
                // TODO compare base type
                // TODO compare interfaces type
                // TODO compare member count
                }
            }

        private void LoadType(Type Type)
            {
            if (Type.BaseType != null && Type.BaseType != typeof(object))
                this.BaseType = Type.BaseType.FullyQualifiedName();
            else
                this.BaseType = null;

            this.Interfaces = Type.GetInterfaces().Convert(Interface => Interface.FullyQualifiedName());

            if (this.Interfaces.Length == 0)
                this.Interfaces = null;
            // TODO load members
            }
        }
    }