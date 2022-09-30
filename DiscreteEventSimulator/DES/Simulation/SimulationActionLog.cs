using System;
using System.Collections.Generic;
using System.Text;

namespace TDFEstimator.DES.Simulation
{
    public class SimulationActionLog
    {
        public string Action { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get { return StartTime + TimeSpan; } }
    }
}
