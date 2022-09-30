using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDFEstimator.DES
{
    public class PriorityQueue<TKey, TValue> : IPriorityQueue<TKey, TValue>
    {
        private int _Count;
        private bool _IsModified;
        SortedDictionary<TKey, TValue> _PriorityQueue;
        public PriorityQueue()
        {
            _PriorityQueue = new SortedDictionary<TKey, TValue>();
        }

        //Adds the specified element with associated priority to the PriorityQueue<TKey,TValue>.
        public void Push(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var _value = value;
            if (!_PriorityQueue.TryGetValue(key, out _value))
            {
                _PriorityQueue.Add(key, value);
            }

            ++_Count;
            _IsModified = true;
        }
        //Returns True if PriorityQueue is Empty
        public bool IsEmpty
        {
            get => EmptyQueue();
        }
        public bool EmptyQueue()
        {
            bool isEmpty = !_PriorityQueue.Any();
            return isEmpty;
        }

        //Returns the minimal element key from the PriorityQueue<TKey,TValue> without removing it.
        public TKey Peek()
        {
            if (_Count == 0)
                throw new InvalidOperationException("PriorityQueue<TKey, TValue> is empty.");

            return _PriorityQueue.First().Key;

        }
        /***Removes and returns the minimal element from the PriorityQueue<TKey,TValue> - 
         that is, the element with the lowest priority value.***/
        public KeyValuePair<TKey, TValue> Pop()
        {
            if (_Count == 0)
                throw new InvalidOperationException("PriorityQueue<TKey, TValue> is empty.");

            KeyValuePair<TKey, TValue> pair = _PriorityQueue.First();
            _PriorityQueue.Remove(pair.Key);
            _IsModified = true;
            return pair;
        }
        /***Removes the minimal element from the PriorityQueue<TElement,TPriority>, 
        and copies it and its associated priority to the element and priority arguments.***/
        public bool TryPop(out KeyValuePair<TKey, TValue> value)
        {
            if (_Count == 0)
                throw new InvalidOperationException("PriorityQueue<TKey, TValue> is empty.");

            value = _PriorityQueue.First();
            _IsModified = true;
            return _PriorityQueue.Remove(value.Key);
        }
    }
}
