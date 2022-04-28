using System;
using System.Collections.Generic;
using System.Linq;

namespace K8sBackendShared.Utils
{
    public class ThreadedQueue<TType>
    {
        private Queue<TType> _queue;
        private object _queueLock;

        public ThreadedQueue()
        {
            _queue = new Queue<TType>();
            _queueLock = new object();
        }

        public void Enqueue(TType data)
        {
            lock (_queueLock)
            {
                // do not allow duplicates
                if (!_queue.Contains(data))
                {
                    _queue.Enqueue(data);

                }

            }
        }

        public bool TryDequeue(out TType data)
        {
            data = default(TType);
            bool success = false;
            lock (_queueLock)
            {
                if (_queue.Count > 0)
                {
                    data = _queue.Dequeue();
                    success = true;
                }
            }
            return success;
        }

        public int CountElements()
        {
            return _queue.Count();
        }

        public List<TType> BatchDequeue()
        {
            List<TType> dequeueList = new List<TType>();

            lock (_queueLock)
            {

                while (TryDequeue(out TType data))
                {
                    dequeueList.Add(data);
                }
            }

            return dequeueList;
        }

        public void BatchEnqueue(List<TType> data)
        {
            lock (_queueLock)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    _queue.Enqueue(data[i]);
                }
            }
        }

        public List<TType> ToList()
        {
            lock (_queueLock)
            {
                return _queue.ToList();
            }
        }

    }
}