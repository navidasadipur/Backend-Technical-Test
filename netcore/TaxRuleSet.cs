using System;
using System.Collections.Generic;

namespace congestion.calculator
{
    public sealed class TaxRuleSet
    {
        public string City { get; set; }
        public int Year { get; set; }
        public int DailyCapSek { get; set; }                // 60
        public int SingleChargeWindowMinutes { get; set; }  // 60
        public List<RateInterval> RateTable { get; set; }   // hours/rates table
        public HashSet<VehicleType> TollFreeVehicles { get; set; } // exception vehicles
        public Func<DateTime, bool> IsTollFreeDate { get; set; }   // exception free dates stractegy
    }
}
