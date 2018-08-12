//-----------------------------------------------------------------------
// <copyright file="EmptyTypeFormatter.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    /// <summary>
    /// A formatter for empty types. It writes no data, and skips all data that is to be read, deserializing a "default" value.
    /// </summary>
    public class EmptyTypeFormatter<T> : EasyBaseFormatter<T>
    {
        protected override void ReadDataEntry(ref T value, string entryName, EntryType entryType, IDataReader reader)
        {
            // Just skip
            reader.SkipEntry();
        }

        protected override void WriteDataEntries(ref T value, IDataWriter writer)
        {
            // Do nothing
        }
    }
}