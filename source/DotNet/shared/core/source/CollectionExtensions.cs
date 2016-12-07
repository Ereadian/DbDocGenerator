//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="CollectionExtensions.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Data collection extensions
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Check given collection is null or empty. If it is, returns true. otherwise return false.
        /// </summary>
        /// <typeparam name="T">type of collection item</typeparam>
        /// <param name="collection">collection to test</param>
        /// <returns>true if collection is null or empty. false: collection has at least one item</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return (collection == null) || (collection.Count < 1);
        }

        /// <summary>
        /// Check given read-only collection is null or empty. If it is, returns true. otherwise return false.
        /// </summary>
        /// <typeparam name="T">type of collection item</typeparam>
        /// <param name="collection">collection to test</param>
        /// <returns>true if collection is null or empty. false: collection has at least one item</returns>
        public static bool IsReadOnlyNullOrEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return (collection == null) || (collection.Count < 1);
        }

        /// <summary>
        /// Copy from dictionary
        /// </summary>
        /// <typeparam name="TKey">type of dictionary key</typeparam>
        /// <typeparam name="TValue">type of dictionary value</typeparam>
        /// <param name="target">target to receive new items</param>
        /// <param name="source">source items to be added</param>
        /// <returns>true: if anything get copied. false: nothing copied</returns>
        /// <exception cref="ArgumentNullException">target is null</exception>
        public static bool AppendFrom<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            bool hasItemCopied = false;
            if (source != null)
            {
                foreach (var pair in source)
                {
                    if (!target.ContainsKey(pair.Key))
                    {
                        target.Add(pair.Key, pair.Value);
                        hasItemCopied = true;
                    }
                }
            }

            return hasItemCopied;
        }

        /// <summary>
        /// Copy from dictionary
        /// </summary>
        /// <typeparam name="TKey">type of dictionary key</typeparam>
        /// <typeparam name="TValue">type of dictionary value</typeparam>
        /// <param name="target">target to receive new items</param>
        /// <param name="source">source items to be added</param>
        /// <returns>true: if anything get copied. false: nothing copied</returns>
        /// <exception cref="ArgumentNullException">target is null</exception>
        public static bool AppendFromReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> target, IReadOnlyDictionary<TKey, TValue> source)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            bool hasItemCopied = false;
            if (source != null)
            {
                foreach (var pair in source)
                {
                    if (!target.ContainsKey(pair.Key))
                    {
                        target.Add(pair.Key, pair.Value);
                        hasItemCopied = true;
                    }
                }
            }

            return hasItemCopied;
        }
    }
}
