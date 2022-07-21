using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
            if (firstOrderRandom) list = list.OrderByRandom();
            return list.Count > 0 ? list[Random.Range(0, list.Count)] : default;
        }

        /// <summary>
        /// Returns a random element in this list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="firstOrderRandom"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandomElement<T>(this List<T> list, bool firstOrderRandom = false)
        {
            if (firstOrderRandom) list = list.OrderByRandom();
            return list[Random.Range(0, list.Count)];
        }

        public static List<T> OrderByRandom<T>(this List<T> list) => list.OrderBy(x => Random.Range(0, list.Count)).ToList();

        public static bool AddIfNotPresent<T>(this List<T> list, T element)
        {
            if (list.Contains(element)) return false;
            
            list.Add(element);
            
            return true;

        }
    }
}