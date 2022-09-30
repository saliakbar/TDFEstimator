using System;
using System.Collections.Generic;
using System.Text;

namespace TDFEstimator.DES
{
    public interface IPriorityQueue<TKey, TValue>
    {
        void Push(TKey key, TValue value);

        bool IsEmpty { get; }

        TKey Peek();

        KeyValuePair<TKey, TValue> Pop();

        bool TryPop(out KeyValuePair<TKey, TValue> value);
    }
}
