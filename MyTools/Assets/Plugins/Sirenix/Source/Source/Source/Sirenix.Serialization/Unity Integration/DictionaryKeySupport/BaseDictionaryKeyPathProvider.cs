//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryKeyPathProvider.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using System.Collections.Generic;

    public abstract class BaseDictionaryKeyPathProvider<T> : IDictionaryKeyPathProvider<T>, IComparer<T>
    {
        public abstract string ProviderID { get; }

        public abstract T GetKeyFromPathString(string pathStr);

        public abstract string GetPathStringFromKey(T key);

        public abstract int Compare(T x, T y);

        int IDictionaryKeyPathProvider.Compare(object x, object y)
        {
            return this.Compare((T)x, (T)y);
        }

        object IDictionaryKeyPathProvider.GetKeyFromPathString(string pathStr)
        {
            return this.GetKeyFromPathString(pathStr);
        }

        string IDictionaryKeyPathProvider.GetPathStringFromKey(object key)
        {
            return this.GetPathStringFromKey((T)key);
        }
    }
}