using System.Collections.Generic;

namespace VBG.Helpers
{
    public static class IntExtensions
    {
        public static bool IsBetween(this int thisInt, int minimumInclusive, int maximumInclusive)
        {
            return thisInt >= minimumInclusive && thisInt <= maximumInclusive;
        }
    }

    public static class ListExtensions
    {
        public static List<T> RemoveDuplicates<T>(this List<T> list)
        {
            List<T> output = new List<T>();
            for(int i = 0; i < list.Count; i++)
            {
                if(output.Count == 0)
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
            if(typeof(T).Equals(typeof(string)))
            {
                foreach (T item in list)
                {
                    if (output.Trim().Length != 0)
                        output += ", ";
                    output += item;
                };
                return output;
            }
            foreach(T item in list){
                if (output.Trim().Length != 0)
                    output += ", ";
                output += item.ToString();
            };
            return output;
        }
    }
}