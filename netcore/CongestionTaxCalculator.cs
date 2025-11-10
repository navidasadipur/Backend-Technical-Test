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

    public int Calculate(IVehicle vehicle, DateTime[] dates)
    {
        var maxTotalFeePerDay = 60;

        DateTime intervalStart = dates[0];
        int tempFee = GetTollFee(intervalStart, vehicle);

        int totalFee = 0;

        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date, vehicle);

            long diffInMillies = date.Millisecond - intervalStart.Millisecond;
            long minutes = diffInMillies / 1000 / 60;

            if (minutes <= 60)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }
        }
        if (totalFee > maxTotalFeePerDay) totalFee = maxTotalFeePerDay;
        return totalFee;
    }

    public static int GetTollFee(DateTime date, IVehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        var timeOnly = TimeOnly.FromDateTime(date);

        return CalculateCongestionTaxBaseOnHour(timeOnly);
    }

    private static int CalculateCongestionTaxBaseOnHour(TimeOnly timeOnly)
    {
        return congestionHourTaxRules
            .FirstOrDefault(rule => rule.StartTime <= timeOnly.ToTimeSpan() && timeOnly.ToTimeSpan() <= rule.EndTime)
            .TaxAmount;
    }

    private struct CongestionHourTaxRule
    {
        public TimeSpan StartTime;
        public TimeSpan EndTime;
        public int TaxAmount;
    }

    private readonly static HashSet<CongestionHourTaxRule> congestionHourTaxRules = new()
    {
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
    };

    private static bool IsTollFreeVehicle(IVehicle vehicle)
    {
        if (vehicle == null) return false;
        VehicleType vehicleType = vehicle.GetVehicleType();
        return TollFreeVehicles.Contains(vehicleType);
    }

    private static bool IsTollFreeDate(DateTime dateTime)
    {
        var dateOnly = DateOnly.FromDateTime(dateTime);

        // Weekend
        if (dateOnly.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return true;
        }

        // Limit scope
        if (dateOnly.Year is not 2013)
        {
            return true;
        }

        // Whole July
        if (dateOnly.Month is 7)
        {
            return true;
        }

        // Holidays + days before holidays
        return IsHolidayOrPreHoliday2013(dateOnly);
    }

    private static bool IsHolidayOrPreHoliday2013(DateOnly date)
    {
        return TollFree2013.Contains(date);
    }

    private static readonly HashSet<DateOnly> TollFree2013 = new()
    {
        new DateOnly(2013, 1, 1),
        new DateOnly(2013, 3, 28),
        new DateOnly(2013, 3, 29),
        new DateOnly(2013, 4, 1),
        new DateOnly(2013, 4, 30),
        new DateOnly(2013, 5, 1),
        new DateOnly(2013, 5, 8),
        new DateOnly(2013, 5, 9),
        new DateOnly(2013, 6, 5),
        new DateOnly(2013, 6, 6),
        new DateOnly(2013, 6, 21),
        // July is toll-free
        new DateOnly(2013, 11, 1),
        new DateOnly(2013, 12, 24),
        new DateOnly(2013, 12, 25),
        new DateOnly(2013, 12, 26),
        new DateOnly(2013, 12, 31)
    };

    private static readonly HashSet<VehicleType> TollFreeVehicles = new()
    {
        VehicleType.Motorcycle,
        VehicleType.Tractor,
        VehicleType.Emergency,
        VehicleType.Diplomat,
        VehicleType.Foreign,
        VehicleType.Military,
    };
}