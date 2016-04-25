﻿using System;
using System.Linq;
using OneHUDData.Vehicle;
using OneHUDData.Timing;

namespace OneHUDData
{
    public class TelemetryData
    {
        public string Game { get; set; }
        public string Description { get; set; }
        public Engine Engine { get; set; }
        public Chassis Chassis { get; set; }
        public Car Car { get; set; }
        public Times Timing { get; set; }  

        private readonly Object _lock = new Object();

        public TelemetryData()
        {
            Reset();
        }

        public void Reset()
        {
            lock (_lock)
            {
                Game = null;
                Engine = new Engine();
                Chassis = new Chassis();
                Car = new Car();
                Timing = new Times();
            }
        }
    }
}
