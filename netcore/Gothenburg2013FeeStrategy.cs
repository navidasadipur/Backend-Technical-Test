using System;
using System.Linq;

namespace congestion.calculator
{
    public class Gothenburg2013FeeStrategy : IGetsFeeByTime
    {
        private readonly static CongestionHourTaxRule[] _congestionHourTaxRules;

        static Gothenburg2013FeeStrategy()
        {
            CongestionHourTaxRule[] rules = [
                new CongestionHourTaxRule { StartTime = new TimeSpan(0, 0, 0), EndTime = new TimeSpan(5, 59, 59), TaxAmount = 0 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(6, 29, 59), TaxAmount = 8 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(6, 30, 0), EndTime = new TimeSpan(6, 59, 59), TaxAmount = 13 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(7, 59, 59), TaxAmount = 18 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 29, 59), TaxAmount = 13 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(14, 59, 59), TaxAmount = 8 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(15, 29, 59), TaxAmount = 13 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 59, 59), TaxAmount = 18 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(17, 59, 59), TaxAmount = 13 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(18, 29, 59), TaxAmount = 8 },
                new CongestionHourTaxRule { StartTime = new TimeSpan(18, 30, 0), EndTime = new TimeSpan(23, 59, 59), TaxAmount = 0 },
            ];

            var rulesDistinctedCount = rules
                .DistinctBy(r => r.StartTime)
                .DistinctBy(r => r.EndTime)
                .Count();

            if (rulesDistinctedCount != rules.Length)
            {
                throw new InvalidOperationException("Congestion hour tax rules contain overlapping time ranges.");
            }

            _congestionHourTaxRules = rules;
        }

        public decimal GetFeeByTime(TimeOnly timeOnly)
        {
            return _congestionHourTaxRules
                .FirstOrDefault(rule => rule.StartTime <= timeOnly.ToTimeSpan() && timeOnly.ToTimeSpan() <= rule.EndTime)
                .TaxAmount;
        }
    }
}


