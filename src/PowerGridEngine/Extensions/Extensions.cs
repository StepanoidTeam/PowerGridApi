using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public static class Extensions
    {
        public static Queue<T> PushItem<T>(this Queue<T> queue, T add)
        {
            queue.Enqueue(add);
            return queue;
        }
    }
}
