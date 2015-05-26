using System;
using System.Collections.Generic;
using System.Linq;

namespace NMonad
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Groups<T>(this IList<T> self, int numberOfGroups)
        {
            int groupSize =(int) Math.Ceiling(((double)self.Count() / (double)numberOfGroups));

            return self.Select((x, i) => Tuple.Create(i, x))
                .GroupBy(x => (x.Item1/groupSize))
                .Select(x => x.Select(y => y.Item2).ToList());

        }

        public static int groupOf(int groupSize, int index, int groupId = 0)
        {
            if (index < groupSize*(groupId + 1))
            {
                return groupId;
            }
            else
            {
                return groupOf(groupSize, index, groupId + 1);
            }
        }
    }
}