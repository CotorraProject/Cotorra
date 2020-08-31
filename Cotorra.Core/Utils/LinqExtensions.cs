using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Cotorra.Core.Utils
{
    public static class LinqExtensions
    {
        public static Task ForEachAsync<TSource>(
     this IEnumerable<TSource> items,
     Func<TSource, Task> action,
     int maxDegreesOfParallelism)
        {
            var actionBlock = new ActionBlock<TSource>(action, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreesOfParallelism
            });

            foreach (var item in items)
            {
                actionBlock.Post(item);
            }

            actionBlock.Complete();

            return actionBlock.Completion;
        }
    }
}
