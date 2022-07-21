using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
    public static void AddIfNoKeyPresent<TKey, TValue>(this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> keyValuePair)
    {
        if (!dict.ContainsKey(keyValuePair.Key))
        {
            dict.Add(keyValuePair.Key, keyValuePair.Value);
        }
    }

    public static void AddIfNoValuePresent<TKey, TValue>(this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> keyValuePair)
    {
        if (!dict.ContainsValue(keyValuePair.Value))
        {
            dict.Add(keyValuePair.Key, keyValuePair.Value);
        }
    }

    public static TKey GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dict) => dict.OrderByRandom().First().Key;
    public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dict) => dict.OrderByRandom().First().Value;
    public static KeyValuePair<TKey, TValue> GetRandomElement<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) => dictionary.OrderByRandom().First();
    public static Dictionary<TKey, TValue> OrderByRandom<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        => dictionary.OrderBy(x => Random.Range(0, dictionary.Count)).ToDictionary(x => x.Key, y => y.Value);
}