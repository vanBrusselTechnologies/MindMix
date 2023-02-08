using System;
using System.Collections.Generic;
using UnityEngine;

namespace VBG.Extensions
{
    public static class IntExt
    {
        public static bool IsBetween(this int i, int minimum, int maximum, bool minimumInclusive = true,
            bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? i >= minimum : i > minimum;
            bool belowMaximum = maximumInclusive ? i <= maximum : i < maximum;
            return aboveMinimum && belowMaximum;
        }
    }

    public static class FloatExt
    {
        public static double Round(this float f, int decimals = 0)
        {
            f -= f * Mathf.Pow(10f, decimals) % 1;
            f /= Mathf.Pow(10f, decimals);
            return f;
        }

        public static bool IsBetween(this float f, float minimum, float maximum, bool minimumInclusive = true,
            bool maximumInclusive = true)
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

    public static class DoubleExt
    {
        public static double Round(this double d, int decimals = 0)
        {
            d -= d * Mathf.Pow(10f, decimals) % 1;
            d /= Mathf.Pow(10f, decimals);
            return d;
        }

        public static bool IsBetween(this double d, double minimum, double maximum, bool minimumInclusive = true,
            bool maximumInclusive = true)
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

    public static class LongExt
    {
        public static bool IsBetween(this long thisLong, long minimum, long maximum, bool minimumInclusive = true,
            bool maximumInclusive = true)
        {
            bool aboveMinimum = minimumInclusive ? thisLong >= minimum : thisLong > minimum;
            bool belowMaximum = maximumInclusive ? thisLong <= maximum : thisLong < maximum;
            return aboveMinimum && belowMaximum;
        }
    }

    public static class ListExt
    {
        /// <summary>
        /// Returns a new list without duplicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RemoveDuplicates<T>(this List<T> list)
        {
            List<T> output = new();
            foreach (var t in list)
            {
                if (output.Count == 0) output.Add(t);
                else if (output.IndexOf(t) == -1) output.Add(t);
            }

            return output;
        }

        public static List<T> GetDuplicates<T>(this List<T> list)
        {
            List<T> tmp = new();
            List<T> output = new();
            foreach (var t in list)
            {
                if (tmp.Count == 0) tmp.Add(t);
                else if (tmp.IndexOf(t) == -1) tmp.Add(t);
                else if (output.IndexOf(t) == -1) output.Add(t);
            }

            return output;
        }

        public static void AddRange<T>(this List<T> list, params List<T>[] items)
        {
            foreach (List<T> l in items)
            {
                foreach (T item in l) list.Add(item);
            }
        }

        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            foreach (var t in items) list.Add(t);
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

        public static void InsertRange<T>(this List<T> list, int index, params T[] items)
        {
            list.AddRange(items);
            for (int i = index; i < list.Count - items.Length; i++)
            {
                list[i + items.Length] = list[i];
            }

            if (index >= list.Count) return;

            for (int i = 0; i < items.Length; i++)
            {
                list[index + i] = items[i];
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
            if (typeof(T) == typeof(string))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T item = list[i];
                    if (output.Trim().Length != 0)
                        output += ",\n";
                    output += "[" + i + "] " + item;
                }

                Debug.Log(output);
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (output.Trim().Length != 0)
                    output += ",\n";
                output += "[" + i + "] " + item;
            }

            Debug.Log(output);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, TValue value)
        {
            Dictionary<TKey, TValue> dict = new();
            foreach (TKey item in keys) dict.Add(item, value);
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, List<TValue> values)
        {
            Dictionary<TKey, TValue> dict = new();
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(List<TKey> keys, params TValue[] values)
        {
            Dictionary<TKey, TValue> dict = new();
            int count = Mathf.Min(keys.Count, values.Length);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
            return dict;
        }
    }

    public static class ArrayExt
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;
            return array;
        }

        public static T[] AddRange<T>(this T[] array, params T[] items)
        {
            Array.Resize(ref array, array.Length + items.Length);
            for (int i = 0; i < items.Length; i++) array[array.Length - items.Length + i] = items[i];
            return array;
        }

        public static T[] AddRange<T>(this T[] array, List<T> items)
        {
            Array.Resize(ref array, array.Length + items.Count);
            for (int i = 0; i < items.Count; i++) array[array.Length - items.Count + i] = items[i];
            return array;
        }

        public static void InsertRange<T>(this T[] array, int index, List<T> items)
        {
            array.AddRange(items);
            for (int i = index; i < array.Length - items.Count; i++) array[i + items.Count] = array[i];

            if (index < array.Length)
            {
                for (int i = 0; i < items.Count; i++) array[index + i] = items[i];
            }
        }

        public static void InsertRange<T>(this T[] array, int index, params T[] items)
        {
            array.AddRange(items);
            for (int i = index; i < array.Length - items.Length; i++)
            {
                array[i + items.Length] = array[i];
            }

            if (index >= array.Length) return;
            for (int i = 0; i < items.Length; i++) array[index + i] = items[i];
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
                if (!array[i].Equals(item)) continue;
                itemCountInArray += 1;
                for (int ii = i; ii < array.Length; ii++) array[ii] = array[ii + 1];
                break;
            }

            Array.Resize(ref array, array.Length - itemCountInArray);
            return array;
        }

        public static T[] RemoveAll<T>(this T[] array, T item)
        {
            int itemCountInArray = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Equals(item)) continue;
                itemCountInArray += 1;
                for (int ii = i; ii < array.Length; ii++) array[ii] = array[ii + 1];
            }

            Array.Resize(ref array, array.Length - itemCountInArray);
            return array;
        }

        public static T[] RemoveRange<T>(this T[] array, int index, int count)
        {
            if (count <= 0) return array;
            int lastItemToRemove = Mathf.Min(array.Length, index + count);
            int itemsToRemove = Mathf.Max(0, lastItemToRemove - index);
            for (int i = index; i < array.Length - itemsToRemove; i++) array[i] = array[i + itemsToRemove];
            Array.Resize(ref array, array.Length - itemsToRemove);
            return array;
        }

        public static T[] Clear<T>(this T[] array)
        {
            Array.Resize(ref array, 0);
            return array;
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            foreach (T value in array)
                if (value.Equals(item))
                    return true;
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
            if (typeof(T) == typeof(string))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    T item = array[i];
                    if (output.Trim().Length != 0)
                        output += ",\n";
                    output += "[" + i + "] " + item;
                }

                Debug.Log(output);
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                T item = array[i];
                if (output.Trim().Length != 0)
                    output += ",\n";
                output += "[" + i + "] " + item;
            }

            Debug.Log(output);
        }

        public static List<T> ToList<T>(this T[] array)
        {
            List<T> output = new();
            foreach (T item in array) output.Add(item);
            return output;
        }

        public static int Count<T>(this T[] array)
        {
            return array.Length;
        }

        public static T[] Resize<T>(this T[] array, int size)
        {
            Array.Resize(ref array, size);
            return array;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, TValue value)
        {
            Dictionary<TKey, TValue> dict = new();
            foreach (TKey item in keys) dict.Add(item, value);
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, List<TValue> values)
        {
            Dictionary<TKey, TValue> dict = new();
            int count = Mathf.Min(keys.Length, values.Count);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
            return dict;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, params TValue[] values)
        {
            Dictionary<TKey, TValue> dict = new();
            int count = Mathf.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
            return dict;
        }
    }

    public static class DictionaryExt
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict,
            Dictionary<TKey, TValue> dictToAdd)
        {
            foreach (KeyValuePair<TKey, TValue> item in dictToAdd) dict.Add(item.Key, item.Value);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys,
            List<TValue> values)
        {
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys,
            params TValue[] values)
        {
            int count = Mathf.Min(keys.Count, values.Length);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys,
            params TValue[] values)
        {
            int count = Mathf.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++) dict.Add(keys[i], values[i]);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys, TValue value)
        {
            foreach (TKey item in keys) dict.Add(item, value);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys, TValue value)
        {
            foreach (TKey item in keys) dict.Add(item, value);
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys)
        {
            if (keys.Count > dict.Count)
            {
                foreach (KeyValuePair<TKey, TValue> item in dict)
                    if (keys.Contains(item.Key))
                        dict.Remove(item.Key);
            }
            else
            {
                foreach (TKey key in keys) dict.Remove(key);
            }
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keys)
        {
            if (keys.Length > dict.Count)
            {
                foreach (KeyValuePair<TKey, TValue> item in dict)
                    if (keys.Contains(item.Key))
                        dict.Remove(item.Key);
            }
            else
                foreach (TKey key in keys)
                    dict.Remove(key);
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, int index, int count)
        {
            index = Mathf.Max(0, index);
            List<TKey> keyList = dict.ToKeyList();
            count = Mathf.Clamp(count, 0, keyList.Count - index);
            for (int i = 0; i < count; i++) dict.Remove(keyList[i + index]);
        }

        public static List<TKey> ToKeyList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<TKey> list = new();
            foreach (KeyValuePair<TKey, TValue> item in dict) list.Add(item.Key);
            return list;
        }

        public static List<TValue> ToValueList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<TValue> list = new();
            foreach (KeyValuePair<TKey, TValue> item in dict) list.Add(item.Value);
            return list;
        }

        public static List<KeyValuePair<TKey, TValue>> ToList<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            List<KeyValuePair<TKey, TValue>> list = new();
            foreach (KeyValuePair<TKey, TValue> item in dict) list.Add(item);
            return list;
        }

        public static int IndexOfKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ToKeyList().IndexOf(key);
        }
    }

    public static class GameObjectExt
    {
        /// <summary>
        /// Checks if the GameObject is in Camera.main's viewport
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInCameraViewport(this GameObject obj)
        {
            return IsInCameraViewport(obj, Camera.main);
        }

        /// <summary>
        /// Checks if the GameObject is in Camera's viewport
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="camera"></param>
        /// <returns>True if the GameObject is in camera's viewport, otherwise false</returns>
        public static bool IsInCameraViewport(this GameObject obj, Camera camera)
        {
            //Works with camera.rotation = Vector3.zero and gameobject.rotation = Vector3.zero;
            Debug.LogWarning("Currently only correct without rotations");
            if (camera == null)
            {
                camera = Camera.main;
            }

            if (camera == null) return false;
            Transform camTf = camera.transform;
            Transform objTf = obj.transform;
            if (camera.orthographic)
            {
                if (camTf.eulerAngles == Vector3.zero)
                {
                    if ((objTf.position.z - (objTf.lossyScale.z / 2f)).IsBetween(
                            camTf.position.z + camera.nearClipPlane, camTf.position.z + camera.farClipPlane, true,
                            false))
                    {
                        if (camTf.position.y + camera.orthographicSize > objTf.position.y - objTf.localScale.y / 2f &&
                            camTf.position.y - camera.orthographicSize < objTf.position.y + objTf.localScale.y / 2f)
                        {
                            if (camTf.position.x + camera.orthographicSize * camera.aspect >
                                objTf.position.x - (objTf.localScale.x / 2f) &&
                                camTf.position.x - camera.orthographicSize * camera.aspect <
                                objTf.position.x + objTf.localScale.x / 2f)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (camTf.eulerAngles == Vector3.zero)
                {
                    if ((objTf.position.z - (objTf.lossyScale.z / 2f)).IsBetween(
                            camTf.position.z + camera.nearClipPlane, camTf.position.z + camera.farClipPlane, true,
                            false))
                    {
                        Debug.LogWarning("Rotation through perspective view has not yet been included.");
                        //Pretty much correct. Only the rotation through perspective view has not yet been included.
                        //1 / Mathf.Tan(camera.fieldOfView / 2f * Mathf.PI / 180f) -> https://www.scratchapixel.com/lessons/3d-basic-rendering/perspective-and-orthographic-projection-matrix/building-basic-perspective-projection-matrix
                        float fovUnits = Mathf.Tan(camera.fieldOfView / 2f * Mathf.PI / 180f) / 50f *
                                         (objTf.position.z - camTf.position.z) * 100f;
                        if (objTf.position.y + (objTf.localScale.y / 2f) > camTf.position.y - fovUnits / 2f &&
                            objTf.position.y - (objTf.localScale.y / 2f) < camTf.position.y + fovUnits / 2f)
                        {
                            if (objTf.position.x + (objTf.localScale.x / 2f) >
                                camTf.position.x - fovUnits * camera.aspect / 2f &&
                                objTf.position.x - (objTf.localScale.x / 2f) <
                                camTf.position.x + fovUnits * camera.aspect / 2f)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="camera"></param>
        /// <returns>0 = completely outside camera view, 1 = completely inside camera view</returns>
        public static float PercentageInCameraView(this GameObject obj, Camera camera)
        {
            Transform camTf = camera.transform;
            Transform objTf = obj.transform;
            Debug.Log(camTf + " " + objTf);
            return 1;
        }
    }

    public static class Vector3Ext
    {
        /// <summary>
        /// Compares two Vector3 values and returns true if they are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }
    }

    /// <summary>
    /// Extension for everything related to UnityEngine.Screen
    /// </summary>
    public static class ScreenExt
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels">Screenpixels you want to convert to Units</param>
        /// <param name="orthographicCam">Orthografic camera used. If not set Camera.main will be used</param>
        /// <returns></returns>
        public static float PixelsToUnits(float pixels, Camera orthographicCam)
        {
            return orthographicCam.orthographicSize * 2 * pixels / Screen.height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="units">Units you want to convert to Screenpixels</param>
        /// <param name="orthographicCam">Orthografic camera used. If not set Camera.main will be used</param>
        /// <returns></returns>
        public static float UnitsToPixels(float units, Camera orthographicCam)
        {
            return units * Screen.height / orthographicCam.orthographicSize / 2;
        }

        public static float BottomOutsideSafezone
        {
            get { return Screen.safeArea.y; }
        }

        public static float LeftOutsideSafezone
        {
            get { return Screen.safeArea.x; }
        }

        public static float RightOutsideSafezone
        {
            get { return Screen.width - Screen.safeArea.width - Screen.safeArea.x; }
        }

        public static float TopOutsideSafezone
        {
            get { return Screen.height - Screen.safeArea.height - Screen.safeArea.y; }
        }

        public static float WidthInUnits
        {
            get { return PixelsToUnits(Screen.width, Camera.main); }
        }

        public static float HeightInUnits
        {
            get { return PixelsToUnits(Screen.height, Camera.main); }
        }

        public static float SafeAreaWidthInUnits
        {
            get { return PixelsToUnits(Screen.safeArea.width, Camera.main); }
        }

        public static float SafeAreaHeightInUnits
        {
            get { return PixelsToUnits(Screen.safeArea.height, Camera.main); }
        }

        /// <summary>
        /// Returns Screen.width divided by Screen.height as float
        /// </summary>
        public static float Aspect
        {
            get { return Screen.width * 1f / Screen.height; }
        }
    }
}