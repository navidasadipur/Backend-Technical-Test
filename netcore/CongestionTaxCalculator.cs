using congestion.calculator;
using System;
using System.Collections.Generic;
using System.Linq;
public class CongestionTaxCalculator
{
    /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total congestion tax for that day
         */

    private readonly TaxRuleSet _rules;
    public CongestionTaxCalculator(TaxRuleSet rules) => _rules = rules;

    public int GetDailyTax(IVehicle vehicle, IEnumerable<DateTime> passes)
    {
        if (vehicle is null) throw new ArgumentNullException(nameof(vehicle));
        if (passes is null) throw new ArgumentNullException(nameof(passes));


        // No tax for toll-free vehicles
        if (_rules.TollFreeVehicles.Contains(vehicle.Type)) return 0;


        var list = passes
            .Where(p => p.Year == _rules.Year)
            .OrderBy(p => p)
            .ToList();
        if (list.Count == 0) return 0;


        if (_rules.IsTollFreeDate(list[0])) return 0;


        // Apply single-charge rule using a sliding window of 60 minutes.
        var windowStart = list[0];
        int windowMax = GetTollFee(list[0]);
        int total = 0;


        for (int i = 1; i < list.Count; i++)
        {
            var current = list[i];
            var minutes = (int)(current - windowStart).TotalMinutes;
            var fee = GetTollFee(current);


            if (minutes <= _rules.SingleChargeWindowMinutes)
            {
                // Still within window → keep only the highest fee in the window
                if (fee > windowMax) windowMax = fee;
            }
            else
            {
                // Close previous window
                total += windowMax;
                if (total >= _rules.DailyCapSek) return _rules.DailyCapSek;


                // Start new window
                windowStart = current;
                windowMax = fee;
            }
        }


        // Add the last window
        total += windowMax;
        if (total > _rules.DailyCapSek) total = _rules.DailyCapSek;
        return total;
    }

    private int GetTollFee(DateTime dt)
    {
        var t = dt.TimeOfDay;
        foreach (var r in _rules.RateTable)
            if (r.Range.Contains(t)) return r.AmountSek;
        return 0;
    }
}
