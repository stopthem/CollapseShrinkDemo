using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace CanTemplate.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a random element in this array.
        ///<para>Note: If length is 0, returns default T.</para>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="firstOrderRandom"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandomElementOrDefault<T>(this T[] array, bool firstOrderRandom = false)
        {
            if (firstOrderRandom) array = array.OrderByRandom();
            return array.Length > 0 ? array[Random.Range(0, array.Length)] : default(T);
        }

        /// <summary>
        /// Returns a random element in this array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="firstOrderRandom"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandomElement<T>(this T[] array, bool firstOrderRandom = false)
        {
            if (firstOrderRandom) array = array.OrderByRandom();
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Randomly orders given array
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] OrderByRandom<T>(this T[] array) => array.OrderBy(x => Random.Range(0, array.Length)).ToArray();
    }
}