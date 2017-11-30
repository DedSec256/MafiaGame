using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MafiaGame.Structures
{
    public class CyclicQueue<T> : IEnumerator<T>
    {
        private T[] queue;
        private int pos;
        public CyclicQueue(T[] arr)
        {
            pos = 0;
            queue = arr;
        }

        public CyclicQueue(IEnumerable<T> arr) : this(arr.ToArray())
        {
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (pos == queue.Length - 1) Reset();
            else ++pos;
            return true;
        }

        public T GetNext()
        {
            MoveNext();
            return Current;
        }

        public void Reset()
        {
            pos = 0;
        }

        public T Current
        {
            get => queue[pos];
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}