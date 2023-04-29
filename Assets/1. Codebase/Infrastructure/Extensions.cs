using System;
using System.Collections.Generic;
using MyBox;
using Random = UnityEngine.Random;

namespace Codebase.Infrastructure
{
    public static class Extensions
    {
        public enum CharComparisonType
        {
            CaseSensitive,
            IgnoreCase,
        }

        public static bool Equals(this char chr1, char chr2, CharComparisonType comparisonType)
        {
            return comparisonType == CharComparisonType.IgnoreCase
                ? char.ToLower(chr1).Equals(char.ToLower(chr2))
                : chr1.Equals(chr2);
        }

        public static bool Contains<T>(this IEnumerable<T> collection, T elem, Func<T, T, bool> predicate)
        {
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate.Invoke(enumerator.Current, elem))
                {
                    return true;
                }
            }

            return false;
        }

        public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue[]> dict)
        {
            var collections = dict.Values;
            var amountOfElems = 0;
            collections.ForEach(collection => amountOfElems += collection.Length);
            var randIndex = Random.Range(0, amountOfElems);
            foreach (var collection in collections)
            {
                var collectionCount = collection.Length;
                if (randIndex >= collectionCount)
                {
                    randIndex -= collectionCount;
                }
                else
                {
                    return collection[randIndex];
                }
            }

            return default;
        }

        public static bool Contains(this Dictionary<char, string[]> dict, string word) =>
            dict.TryGetValue(word[0], out var words, CharComparisonType.IgnoreCase) && words.Contains(word,
                (val1, val2) => val1.Equals(val2, StringComparison.CurrentCultureIgnoreCase));

        public static bool TryGetValue<TValue>(this Dictionary<char, TValue> dict, char key, out TValue value, CharComparisonType comparisonType)
        {
            foreach (var pair in dict)
            {
                if (pair.Key.Equals(key, comparisonType))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}