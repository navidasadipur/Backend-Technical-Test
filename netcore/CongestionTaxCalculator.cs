using System;
using System.Collections.Generic;

namespace congestion.calculator;

public class CongestionTaxCalculator
{
    private int Strategy { get; set; }

    //public CongestionTaxCalculator(Strategy strategy)
    //{
    //    Strategy = strategy;
    //}

    /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total congestion tax for that day
         */

    /// <summary>
    /// strategyClassName: The name of the congestion fee strategy class to use (e.g., "Gothenburg2013FeeStrategy").
    /// vehicle: An instance of IVehicle representing the vehicle for which the toll fee is being calculated.
    /// dates: An array of DateTime objects representing the date and time of all passes on one day.
    /// </summary>
    public int Calculate(string strategyClassName, IVehicle vehicle, DateTime[] dates)
    {
        var maxTotalFeePerDay = 60;

        DateTime intervalStart = dates[0];
        int tempFee = GetTollFee(strategyClassName, intervalStart, vehicle);

        int totalFee = 0;

        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(strategyClassName, date, vehicle);

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

    public static int GetTollFee(string strategyClassName, DateTime date, IVehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        var timeOnly = TimeOnly.FromDateTime(date);

        return CalculateCongestionTaxByTime(strategyClassName, timeOnly);
    }

    private static int CalculateCongestionTaxByTime(string strategyClassName, TimeOnly timeOnly)
    {

        IGetsFeeByTime feeStrategy = strategyClassName switch
        {
            nameof(Gothenburg2013FeeStrategy) => new Gothenburg2013FeeStrategy(),
            _ => throw new NotSupportedException($"Fee strategy '{strategyClassName}' is not supported.")
        };
        return (int)feeStrategy.GetFeeByTime(timeOnly);
    }

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