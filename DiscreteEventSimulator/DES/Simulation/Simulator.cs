using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TDFEstimator.DES.Simulation
{
    public class Simulator : IDiscreteEventSimulator
    {
        private readonly Dictionary<Thread, SimulationThreadInfo> _threads = new Dictionary<Thread, SimulationThreadInfo>();
        private readonly PriorityQueue<TimeSpan, SimulationEvent> _timeLine = new PriorityQueue<TimeSpan, SimulationEvent>();
        private readonly ManualResetEventSlim _completionEvent = new ManualResetEventSlim();
        private readonly object _lock = new object();
        public TimeSpan Time { get; private set; }
        public List<SimulationActionLog> SimulationActionLogs { get; set; }
        public DateTime SimulationStartTime { get; set; }

        public Simulator()
        {
            SimulationActionLogs = new List<SimulationActionLog>();
            SimulationStartTime = DateTime.Now;
        }


        

        public void AddThread(Thread thread)
        {
            _threads.Add(thread, new SimulationThreadInfo());
        }

        public void Delay(TimeSpan delay)
        {
            Delay(delay, Thread.CurrentThread);
        }

        public void Delay(TimeSpan delay, Thread thread)
        {
            _threads[thread].Delay(this, delay);
        }

        public void Delay(double delay)
        {
            Delay(delay, Thread.CurrentThread);
        }

        public SimulationActionLog AddSimulationActionLogAndDelay(string command, double delay)
        {
            TimeSpan timeSpanElapsed = _threads[Thread.CurrentThread].GetCurrentTime();
            TimeSpan timeSpan2 = GetTimeSpan(delay);
            SimulationActionLog simActLog = new SimulationActionLog();
            simActLog.Action = command;
            simActLog.TimeSpan = timeSpan2;
            simActLog.StartTime = this.SimulationStartTime + timeSpanElapsed;
            this.SimulationActionLogs.Add(simActLog);
            this.Delay(timeSpan2);
            return simActLog;
        }

        public void Delay(double delay, Thread thread)
        {
            TimeSpan timeSpan = GetTimeSpan(delay);
            _threads[thread].Delay(this, timeSpan);
        }

        internal void TryAdvanceSimulationTime(SimulationThreadInfo threadinfo)
        {
            lock (_lock)
            {
                if (_threads.Count > 0)
                {
                    var nextTime = threadinfo == null ? _threads.Values.Min(t => t.Time) : threadinfo.Time;
                    while ((!_timeLine.IsEmpty) && (_timeLine.Peek() <= nextTime))
                    {
                        var nextEvent = _timeLine.Pop();
                        Time = nextEvent.Key;
                        nextEvent.Value.Release();
                    }
                    if (_timeLine.IsEmpty)
                    {
                        _completionEvent.Set();
                    }
                }
            }
        }

        internal void AdvanceSimulationTime()
        {
            while (!_timeLine.IsEmpty)
            {
                var nextEvent = _timeLine.Pop();
                Time = nextEvent.Key;
                nextEvent.Value.Release();
            }
        }

        public TimeSpan GetTime()
        {
            return GetTime(Thread.CurrentThread);
        }

        public void PauseWatch()
        {
            _threads[Thread.CurrentThread].PauseWatch();
        }
        public void ResumeWatch()
        {
            _threads[Thread.CurrentThread].ResumeWatch();
        }

        public TimeSpan GetTime(Thread thread)
        {
            return _threads[thread].GetCurrentTime();
        }

        public void RemoveThread()
        {
            _threads.Remove(Thread.CurrentThread);
            TryAdvanceSimulationTime(null);
        }

        public void Start()
        {
            foreach (var thread in _threads.Values)
            {
                thread.Initialize();
            }
        }
        public void Log(string message, params object[] args)
        {
            Log(Thread.CurrentThread, message, args);
        }

        public void Log(Thread thread, string message, params object[] args)
        {
            _threads[thread].Log(message, args);
        }

        public TimeSpan Stop()
        {
            while (!_timeLine.IsEmpty)
            {
                _completionEvent.Wait();
            }
            return Time;
        }

        internal void PushEvent(SimulationEvent ev)
        {
            lock (_lock)
            {
                _timeLine.Push(ev.Time, ev);
            }
        }

        public TimeSpan GetTimeSpan(double delaySeconds)
        {
            int seconds = Convert.ToInt32(Math.Truncate(delaySeconds));
            int milliseconds = Convert.ToInt32(Math.Truncate((delaySeconds - seconds) * 1000));
            TimeSpan delay = new TimeSpan(0, 0, 0, seconds, milliseconds);
            return delay;
        }
    }
}
