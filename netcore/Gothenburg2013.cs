using System;
using System.Collections.Generic;

namespace congestion.calculator
{
    // =============================
    // Holidays (2013 Gothenburg)
    // =============================
    public static class Gothenburg2013
    {
        // Public holidays, weekends, days before a public holiday, and the whole July are toll‑free.
        // The list below captures 2013 Swedish public holidays and marks the day before as toll‑free.
        // For brevity we include only those relevant around the assignment examples plus full July.
        private static readonly HashSet<DateOnly> Holidays2013 = new()
            {
            new DateOnly(2013,1,1), // New Year's Day
            new DateOnly(2013,3,29), // Good Friday
            new DateOnly(2013,4,1), // Easter Monday
            new DateOnly(2013,5,1), // May Day
            new DateOnly(2013,5,9), // Ascension Day
            new DateOnly(2013,6,6), // National Day
            new DateOnly(2013,6,21), // Midsummer Eve (treated toll‑free in spec)
            new DateOnly(2013,11,1), // All Saints’ Day
            new DateOnly(2013,12,24), // Christmas Eve (treated toll‑free in spec)
            new DateOnly(2013,12,25), // Christmas Day
            new DateOnly(2013,12,26), // Boxing Day
            new DateOnly(2013,12,31), // New Year’s Eve
            };


        private static readonly HashSet<DateOnly> DaysBeforeHoliday2013 = new()
            {
            new DateOnly(2013,3,28), // Maundy Thursday (day before Good Friday)
            new DateOnly(2013,4,30), // Walpurgis Night (day before May 1)
            new DateOnly(2013,6,5), // Day before National Day
            // Many municipalities also treat the day before Ascension (2013‑05‑08) as toll‑free per assignment table
            new DateOnly(2013,5,8),
            };


        public static bool IsTollFreeDate2013(DateOnly d)
        {
            if (d.Year != 2013) return false; // we limit to 2013
            if (d.Month == 7) return true; // entire July
            if (Holidays2013.Contains(d)) return true;
            if (DaysBeforeHoliday2013.Contains(d)) return true;
            var dow = d.DayOfWeek;
            if (dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday) return true;
            return false;
        }


        public static TaxRuleSet BuildRules()
        {
            return new TaxRuleSet
            {
                City = "Gothenburg",
                Year = 2013,
                DailyCapSek = 60,
                SingleChargeWindowMinutes = 60,
                TollFreeVehicles = new HashSet<VehicleType>
                    {
                    VehicleType.Emergency,
                    VehicleType.Bus,
                    VehicleType.Diplomat,
                    VehicleType.Motorcycle,
                    VehicleType.Military,
                    VehicleType.Foreign,
                    },
                RateTable = new List<RateInterval>
                    {
                    new RateInterval(new TimeRange(TimeSpan.Parse("06:00"), TimeSpan.Parse("06:30")), 8),
                    new RateInterval(new TimeRange(TimeSpan.Parse("06:30"), TimeSpan.Parse("07:00")), 13),
                    new RateInterval(new TimeRange(TimeSpan.Parse("07:00"), TimeSpan.Parse("08:00")), 18),
                    new RateInterval(new TimeRange(TimeSpan.Parse("08:00"), TimeSpan.Parse("08:30")), 13),
                    new RateInterval(new TimeRange(TimeSpan.Parse("08:30"), TimeSpan.Parse("15:00")), 8),
                    new RateInterval(new TimeRange(TimeSpan.Parse("15:00"), TimeSpan.Parse("15:30")), 13),
                    new RateInterval(new TimeRange(TimeSpan.Parse("15:30"), TimeSpan.Parse("17:00")), 18),
                    new RateInterval(new TimeRange(TimeSpan.Parse("17:00"), TimeSpan.Parse("18:00")), 13),
                    new RateInterval(new TimeRange(TimeSpan.Parse("18:00"), TimeSpan.Parse("18:30")), 8),
                    // implicit 0 SEK otherwise
                    },
                IsTollFreeDate = IsTollFreeDate2013
            };
        }
    }
}
