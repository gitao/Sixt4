using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sixt4.TestAssignment
{
    public class ConcurrentQueue<T>
    {
        private Queue<T> list = new Queue<T>();

        public void Enqueue(T value)
        {
            lock (list)
            {
                list.Enqueue(value);
                Monitor.Pulse(list);
            }
        }

        public void Enqueue(List<T> values)
        {
            foreach (T value in values)
            {
                Enqueue(value);
            }
        }

        public T Dequeue(int millisecondTimeout)
        {
            lock (list)
            {
                if (list.Count() == 0)
                {
                    if (!Monitor.Wait(list, millisecondTimeout))
                    {
                        throw new TimeoutException();
                    }
                }

                T value = list.Dequeue();

                return value;
            }
        }


        public List<T> DequeueAll(int millisecondTimeout)
        {
            List<T> values = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                values.Add(Dequeue(millisecondTimeout));
            }

            return values;
        }

        public int Count()
        {
            return list.Count;
        }
    }
}
