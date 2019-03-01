using System;
using System.Collections.Generic;
using System.Text;

namespace IoTBeaconScans
{
    public class BeaconState
    {
        public Int32 RSSI { get; set; }

        public int BatteryLevel { get; set; }

        public double Humidity { get; set; }

        public int Temperature { get; set; }
        public double CoordLat { get; set; }
        public double CoordLong { get; set; }
    }
}
