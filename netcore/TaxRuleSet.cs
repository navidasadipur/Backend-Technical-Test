using System;
using System.Collections.Generic;

namespace congestion.calculator
{
    public sealed class TaxRuleSet
    {
        public required string City { get; init; }
        public required int Year { get; init; }
        public required int DailyCapSek { get; init; }                // 60
        public required int SingleChargeWindowMinutes { get; init; }  // 60
        public required List<RateInterval> RateTable { get; init; }   // hours/rates table
        public required HashSet<VehicleType> TollFreeVehicles { get; init; } // exception vehicles
        public required Func<DateOnly, bool> IsTollFreeDate { get; init; }   // exception free dates stractegy
    }
}
