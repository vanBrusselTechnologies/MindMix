using System.Collections.Generic;
using UnityEngine;

namespace VBG.Extensions
{
    public static class Int
    {
        public static bool IsBetween(this int i, int minimum, int maximum, bool minimumInclusive = true, bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? i >= minimum : i > minimum;
            bool belowMaximum = maximumInclusive ? i <= maximum : i < maximum;
            return aboveMinimum && belowMaximum;
        }
    }

    public static class Float
    {
        public static double Round(this float f, int decimals = 0)
        {
            f -= f * Mathf.Pow(10f, decimals) % 1;
            f /= Mathf.Pow(10f, decimals);
            return f;
        }

        public static bool IsBetween(this float f, float minimum, float maximum, bool minimumInclusive = true, bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? f >= minimum : f > minimum;
            bool belowMaximum = maximumInclusive ? f <= maximum : f < maximum;
            return aboveMinimum && belowMaximum;
        }

        public static int ToInt(this float f)
        {
            return (int)f;
        }

        public static long ToLong(this float f)
        {
            return (long)f;
        }

        //public static 
    }

    public static class Double
    {
        public static double Round(this double d, int decimals = 0)
        {
            d -= d * Mathf.Pow(10f, decimals) % 1;
            d /= Mathf.Pow(10f, decimals);
            return d;
        }

        public static bool IsBetween(this double d, double minimum, double maximum, bool minimumInclusive = true, bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? d >= minimum : d > minimum;
            bool belowMaximum = maximumInclusive ? d <= maximum : d < maximum;
            return aboveMinimum && belowMaximum;
        }

        public static int ToInt(this double d)
        {
            return (int)d;
        }

        public static long ToLong(this double d)
        {
            return (long)d;
        }

        public static float ToFloat(this double d)
        {
            return (float)d;
        }
    }

    public static class Long
    {
        public static bool IsBetween(this long thisLong, long minimum, long maximum, bool minimumInclusive = true, bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? thisLong >= minimum : thisLong > minimum;
            bool belowMaximum = maximumInclusive ? thisLong <= maximum : thisLong < maximum;
            return aboveMinimum && belowMaximum;
        }
    }

    public static class List
    {
        /// <summary>
        /// Returns a new list without duplicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RemoveDuplicates<T>(this List<T> list)
        {
            List<T> output = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (output.Count == 0)
                {
                    output.Add(list[i]);
                }
                else if (output.IndexOf(list[i]) == -1)
                {
                    output.Add(list[i]);
                }
            }
            return output;
        }

        public static List<T> GetDuplicates<T>(this List<T> list)
        {
            List<T> tmp = new List<T>();
            List<T> output = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (tmp.Count == 0)
                {
                    tmp.Add(list[i]);
                }
                else if (tmp.IndexOf(list[i]) == -1)
                {
                    tmp.Add(list[i]);
                }
                else if (output.IndexOf(list[i]) == -1)
                {
                    output.Add(list[i]);
                }
            }
            return output;
        }

        public static void AddRange<T>(this List<T> list, List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                list.Add(items[i]);
            }
        }

        public static void AddRange<T>(this List<T> list, T[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                list.Add(items[i]);
            }
        }

        public static void InsertRange<T>(this List<T> list, int index, List<T> items)
        {
            list.AddRange(items);
            for (int i = index; i < list.Count - items.Count; i++)
            {
                list[i + items.Count] = list[i];
            }
            if (index < list.Count)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    list[index + i] = items[i];
                }
            }
        }

        public static void InsertRange<T>(this List<T> list, int index, T[] items)
        {
            list.AddRange(items);
            for (int i = index; i < list.Count - items.Length; i++)
            {
                list[i + items.Length] = list[i];
            }
            if (index < list.Count)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    list[index + i] = items[i];
                }
            }
        }

        /// <summary>
        /// Logs all items of the list to the console
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void LogItems<T>(this List<T> list)
        {
            string output = "";
            if (typeof(T).Equals(typeof(string)))
            {
                for (int i  = 0; i < list.Count; i++)
                {
                    T item = list[i];
                    if (output.Trim().Length != 0)
                        output += ",\n";
                    output += "[" + i + "] " + item;
                };
                Debug.Log(output);
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (output.Trim().Length != 0)
                    output += ",\n";
                output += "[" + i + "] " + item.ToString();
            };
            Debug.Log(output);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, TValue value)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (TKey item in keys)
            {
                dict.Add(item, value);
            }
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, List<TValue> values)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, TValue[] values)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            int count = Mathf.Min(keys.Count, values.Length);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            return dict;
        }
    }

    public static class Array
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            System.Array.Resize(ref array, array.Length + 1);
            array[^1] = item;
            return array;
        }

        public static T[] AddRange<T>(this T[] array, T[] items)
        {
            System.Array.Resize(ref array, array.Length + items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                array[array.Length - items.Length + i] = items[i];
            }
            return array;
        }

        public static T[] AddRange<T>(this T[] array, List<T> items)
        {
            System.Array.Resize(ref array, array.Length + items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                array[array.Length - items.Count + i] = items[i];
            }
            return array;
        }

        public static void InsertRange<T>(this T[] array, int index, List<T> items)
        {
            array.AddRange(items);
            for (int i = index; i < array.Length - items.Count; i++)
            {
                array[i + items.Count] = array[i];
            }
            if (index < array.Length)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    array[index + i] = items[i];
                }
            }
        }

        public static void InsertRange<T>(this T[] array, int index, T[] items)
        {
            array.AddRange(items);
            for (int i = index; i < array.Length - items.Length; i++)
            {
                array[i + items.Length] = array[i];
            }
            if (index < array.Length)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    array[index + i] = items[i];
                }
            }
        }

        public static T[] Insert<T>(this T[] array, T item, int index)
        {
            array = array.Add(item);
            for (int i = index; i < array.Length - 1; i++)
            {
                array[i + 1] = array[i];
            }
            array[index] = item;
            return array;
        }

        public static T[] Remove<T>(this T[] array, T item)
        {
            int itemCountInArray = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if(array[i].Equals(item))
                {
                    itemCountInArray += 1;
                    for (int ii = i; ii < array.Length; ii++)
                    {
                        array[ii] = array[ii + 1];
                    }
                    break;
                }
            }
            System.Array.Resize(ref array, array.Length - itemCountInArray);
            return array;
        }

        public static T[] RemoveAll<T>(this T[] array, T item)
        {
            int itemCountInArray = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                {
                    itemCountInArray += 1;
                    for (int ii = i; ii < array.Length; ii++)
                    {
                        array[ii] = array[ii + 1];
                    }
                }
            }
            System.Array.Resize(ref array, array.Length - itemCountInArray);
            return array;
        }

        public static T[] RemoveRange<T>(this T[] array, int index, int count)
        {
            if (count <= 0) return array;
            int lastItemToRemove = Mathf.Min(array.Length, index + count);
            int itemsToRemove = Mathf.Max(0, lastItemToRemove - index);
            for (int i = index; i < array.Length - itemsToRemove; i++)
            {
                array[i] = array[i + itemsToRemove];
            }
            System.Array.Resize(ref array, array.Length - itemsToRemove);
            return array;
        }

        public static T[] Clear<T>(this T[] array)
        {
            System.Array.Resize(ref array, 0);
            return array;
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            foreach(T value in array)
            {
                if (value.Equals(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Logs all items of the array to the console
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void LogItems<T>(this T[] array)
        {
            string output = "";
            if (typeof(T).Equals(typeof(string)))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    T item = array[i];
                    if (output.Trim().Length != 0)
                        output += ",\n";
                    output += "[" + i + "] " + item;
                };
                Debug.Log(output);
                return;
            }
            for (int i = 0; i < array.Length; i++)
            {
                T item = array[i];
                if (output.Trim().Length != 0)
                    output += ",\n";
                output += "[" + i + "] " + item.ToString();
            };
            Debug.Log(output);
        }

        public static List<T> ToList<T>(this T[] array)
        {
            List<T> output = new List<T>();
            foreach(T item in array)
            {
                output.Add(item);
            }
            return output;
        }

        public static int Count<T>(this T[] array)
        {
            return array.Length;
        }

        public static T[] Resize<T>(this T[] array, int size)
        {
            System.Array.Resize(ref array, size);
            return array;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, TValue value)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (TKey item in keys)
            {
                dict.Add(item, value);
            }
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, List<TValue> values)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            int count = Mathf.Min(keys.Length, values.Count);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, TValue[] values)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            int count = Mathf.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            return dict;
        }
    }

    public static class Dictionary
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> dictToAdd)
        {
            foreach(KeyValuePair<TKey, TValue> item in dictToAdd)
            {
                dict.Add(item.Key, item.Value);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys, List<TValue> values)
        {
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys, TValue[] values)
        {
            int count = Mathf.Min(keys.Count, values.Length);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys, TValue[] values)
        {
            int count = Mathf.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys, TValue value)
        {
            foreach (TKey item in keys)
            {
                dict.Add(item, value);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys, TValue value)
        {
            foreach (TKey item in keys)
            {
                dict.Add(item, value);
            }
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys)
        {
            if (keys.Count > dict.Count)
            {
                foreach (KeyValuePair<TKey, TValue> item in dict)
                {
                    if (keys.Contains(item.Key))
                    {
                        dict.Remove(item.Key);
                    }
                }
            }
            else
            {
                foreach(TKey key in keys)
                {
                    dict.Remove(key);
                }
            }
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys)
        {
            if (keys.Length > dict.Count)
            {
                foreach (KeyValuePair<TKey, TValue> item in dict)
                {
                    if (keys.Contains(item.Key))
                    {
                        dict.Remove(item.Key);
                    }
                }
            }
            else
            {
                foreach (TKey key in keys)
                {
                    dict.Remove(key);
                }
            }
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, int index, int count)
        {
            index = Mathf.Max(0, index);
            List<TKey> keyList = dict.ToKeyList();
            count = Mathf.Clamp(count, 0, keyList.Count - index);
            for(int i = 0; i < count; i++)
            {
                dict.Remove(keyList[i + index]);
            }
        }

        public static List<TKey> ToKeyList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<TKey> list = new List<TKey>();
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                list.Add(item.Key);
            }
            return list;
        }

        public static List<TValue> ToValueList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<TValue> list = new List<TValue>();
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                list.Add(item.Value);
            }
            return list;
        }

        public static List<KeyValuePair<TKey,TValue>> ToList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                list.Add(item);
            }
            return list;
        }

        public static int IndexOfKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ToKeyList().IndexOf(key);
        }
    }
}