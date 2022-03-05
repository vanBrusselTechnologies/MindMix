using System.Collections.Generic;

namespace VBG
{
    namespace Extensions
    {
        public static class Int
        {
            public static bool IsBetween(this int thisInt, int minimumInclusive, int maximumInclusive)
            {
                return thisInt >= minimumInclusive && thisInt <= maximumInclusive;
            }
        }

        public static class List
        {
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

            public static string ToItemListString<T>(this List<T> list)
            {
                string output = "";
                if (typeof(T).Equals(typeof(string)))
                {
                    foreach (T item in list)
                    {
                        if (output.Trim().Length != 0)
                            output += ", ";
                        output += item;
                    };
                    return output;
                }
                foreach (T item in list)
                {
                    if (output.Trim().Length != 0)
                        output += ", ";
                    output += item.ToString();
                };
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
                    array[array.Length - 1 - items.Length + i] = items[i];
                }
                return array;
            }
            public static T[] AddRange<T>(this T[] array, List<T> items)
            {
                System.Array.Resize(ref array, array.Length + items.Count);
                for (int i = 0; i < items.Count; i++)
                {
                    array[array.Length - 1 - items.Count + i] = items[i];
                }
                return array;
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
        }
    }
}