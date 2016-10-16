using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source, int size)
        {
            while (source.Any())
            {
                yield return source.Take(size);
                source = source.Skip(size);
            }
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                Partitioner.Create(source).GetPartitions(dop).Select(partition => Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                })));
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }

            return source;
        }
    }
}