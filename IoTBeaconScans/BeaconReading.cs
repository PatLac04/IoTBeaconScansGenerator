using System;
using System.Collections.Generic;
using System.Text;

namespace IoTBeaconScans
{
    public class BeaconReading
    {
        public double Version { get; set; }

        public string CompoundKey { get; set; }

        public int BeaconId { get; set; }

        public int GatewayId { get; set; }

        public string ClientId { get; set; }

        public BeaconState State { get; set; }
        public DateTime EventDatetime { get; set; }
        public long SubmitDatetime { get; set; }
        public string SubmitDay { get; set; }

        public string SubmitHour { get; set; }

        public string SubmitMinute { get; set; }

        public string SubmitSecond { get; set; }
    }
}
