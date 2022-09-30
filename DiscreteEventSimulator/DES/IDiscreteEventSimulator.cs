using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TDFEstimator.DES
{
    public interface IDiscreteEventSimulator
    {
        TimeSpan GetTime();

        TimeSpan GetTime(Thread thread);

        void AddThread(Thread thread);

        void RemoveThread();

        void Delay(TimeSpan delay);

        void Delay(TimeSpan delay, Thread thread);

        void Start();

        TimeSpan Stop();
    }
}
