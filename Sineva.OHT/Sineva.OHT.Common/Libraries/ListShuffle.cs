using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    static public class ListShuffle
    {
        private static Random _Random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n -= 1;
                int k = _Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            //if (newIndex > oldIndex) newIndex--;

            list.Insert(newIndex, item);
        }
    }
}
