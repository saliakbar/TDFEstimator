using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TDFEstimator.DES.Simulation
{
    internal class SimulationThreadInfo
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TimeSpan Time { get; private set; }

        public TimeSpan GetCurrentTime()
        {
            return Time + _stopwatch.Elapsed;
        }

        public void Initialize()
        {
            _stopwatch.Start();
        }

        public void PauseWatch()
        {
            _stopwatch.Stop();
        }
        public void ResumeWatch()
        {
            _stopwatch.Start();
        }

        public void Delay(Simulator simulator, TimeSpan delay)
        {
            _stopwatch.Stop();
            var endTime = Time + _stopwatch.Elapsed + delay;
            Time = endTime;
            var ev = new SimulationEvent(endTime, this);
            simulator.PushEvent(ev);
            simulator.TryAdvanceSimulationTime(this);
            ev.Wait();
            _stopwatch.Restart();
        }

        public void Log(string message, params object[] args)
        {
            StringBuilder timeString = new StringBuilder("Simulation Time Elapsed: ");
            timeString.Append(this.GetCurrentTime().Minutes);
            timeString.Append(" Minutes ");
            timeString.Append(this.GetCurrentTime().Seconds);
            timeString.Append(" Seconds. ");
            Console.WriteLine(timeString + message, args);
        }


        public void AdvanceTo(TimeSpan time)
        {
            Time = time;
        }
    }
}
