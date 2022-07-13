using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CanTemplate.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
            return list;
        }

        /// <summary>
        /// Returns a random element in this list.
        ///<para>Note: If length is 0, returns default T.</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="firstOrderRandom"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandomElementOrDefault<T>(this List<T> list, bool firstOrderRandom = false)
        {
            if (firstOrderRandom) list = list.OrderBy(x => Random.Range(0, list.Count)).ToList();
            return list.Count > 0 ? list[Random.Range(0, list.Count)] : default(T);
        }
    }
}