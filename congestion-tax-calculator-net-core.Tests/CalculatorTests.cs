using congestion.calculator;

namespace congestion_tax_calculator_net_core.Tests;

public class CalculatorTests
{
    //private static TaxRuleSet Rules => Gothenburg2013.BuildRules();
    //private static CongestionTaxCalculator Calc => new(Rules);

    private static IVehicle Car() => new Car();

    // 1) جدول نرخ‌ها: لبه‌ها دقیق باشند
    [Theory]
    [InlineData("06:00", 8)]
    [InlineData("06:29", 8)]
    [InlineData("06:30", 13)]
    [InlineData("06:59", 13)]
    [InlineData("07:00", 18)]
    [InlineData("07:59", 18)]
    [InlineData("08:00", 13)]
    [InlineData("08:29", 13)]
    [InlineData("08:30", 8)]
    [InlineData("14:59", 8)]
    [InlineData("15:00", 13)]
    [InlineData("15:29", 13)]
    [InlineData("15:30", 18)]
    [InlineData("16:59", 18)]
    [InlineData("17:00", 13)]
    [InlineData("17:59", 13)]
    [InlineData("18:00", 8)]
    [InlineData("18:29", 8)]
    [InlineData("18:30", 0)] // خارج از همه بازه‌های پولی
    public void Fee_Table_Boundaries_Are_Correct(string hhmm, int expected)
    {
        var dt = DateTime.Parse($"2013-02-06 {hhmm}:00"); // یک روز کاری عادی
        var fee = CongestionTaxCalculator.GetTollFee(nameof(Gothenburg2013FeeStrategy), dt, Car());
        Assert.Equal(expected, fee);
    }

    //// 2) قانون 60 دقیقه: فقط بیشترین عوارض پنجره حساب شود
    //[Fact]
    //public void SingleChargeWindow_Takes_Highest_Within_60_Minutes()
    //{
    //    // سه عبور در ≤60 دقیقه: 06:05(8), 06:35(13), 06:55(13) => فقط 13
    //    var passes = new[]
    //    {
    //        DateTime.Parse("2013-02-06 06:05:00"),
    //        DateTime.Parse("2013-02-06 06:35:00"),
    //        DateTime.Parse("2013-02-06 06:55:00"),
    //    };
    //    var tax = Calc.GetDailyTax(Car(), passes);
    //    Assert.Equal(13, tax);
    //}

    //// 3) خاتمه پنجره و شروع پنجره جدید
    //[Fact]
    //public void SingleChargeWindow_Closes_After_60_Minutes()
    //{
    //    // 06:05(8) و 07:10(18) با فاصله > 60 دقیقه => 8 + 18 = 26
    //    var passes = new[]
    //    {
    //        DateTime.Parse("2013-02-06 06:05:00"),
    //        DateTime.Parse("2013-02-06 07:10:00"),
    //    };
    //    var tax = Calc.GetDailyTax(Car(), passes);
    //    Assert.Equal(26, tax);
    //}

    //// 4) سقف روزانه
    //[Fact]
    //public void Daily_Cap_Is_60()
    //{
    //    // عبورهای متعدد که مجموع خام > 60 می‌شود => خروجی دقیقاً 60
    //    var passes = new[]
    //    {
    //        DateTime.Parse("2013-02-08 06:20:00"), // 8
    //        DateTime.Parse("2013-02-08 07:10:00"), // 18
    //        DateTime.Parse("2013-02-08 08:10:00"), // 13
    //        DateTime.Parse("2013-02-08 15:10:00"), // 13
    //        DateTime.Parse("2013-02-08 15:40:00"), // 18
    //        DateTime.Parse("2013-02-08 16:45:00"), // 18 (با 15:40 در یک پنجره می‌ماند، بیشینه 18)
    //        DateTime.Parse("2013-02-08 17:10:00"), // 13 (پنجره جدید)
    //        DateTime.Parse("2013-02-08 18:10:00"), // 8
    //    };
    //    var tax = Calc.GetDailyTax(Car(), passes);
    //    Assert.Equal(60, tax);
    //}

    //// 5) تاریخ‌های معاف
    //[Theory]
    //[InlineData("2013-07-10 08:15:00")] // جولای = معاف
    //[InlineData("2013-03-29 08:15:00")] // Good Friday = معاف
    //[InlineData("2013-03-28 14:07:27")] // قبل از Good Friday = معاف
    //[InlineData("2013-11-02 10:00:00")] // شنبه = معاف
    //[InlineData("2013-11-03 10:00:00")] // یکشنبه = معاف
    //public void TollFree_Dates_Return_Zero(string dateTime)
    //{
    //    var dt = DateTime.Parse(dateTime);
    //    var tax = Calc.GetDailyTax(Car(), new[] { dt });
    //    Assert.Equal(0, tax);
    //}

    //// 6) مجموعه تاریخ‌های Post-it شما (خروجی‌های مورد انتظار محاسبه شده)
    //[Fact]
    //public void PostIt_Sample_Days_Match_Expected_Totals()
    //{
    //    var samples = new[]
    //    {
    //        "2013-01-14 21:00:00",
    //        "2013-01-15 21:00:00",
    //        "2013-02-07 06:23:27",
    //        "2013-02-07 15:27:00",
    //        "2013-02-08 06:27:00",
    //        "2013-02-08 06:20:27",
    //        "2013-02-08 14:35:00",
    //        "2013-02-08 15:29:00",
    //        "2013-02-08 15:47:00",
    //        "2013-02-08 16:01:00",
    //        "2013-02-08 16:48:00",
    //        "2013-02-08 17:49:00",
    //        "2013-02-08 18:29:00",
    //        "2013-02-08 18:35:00",
    //        "2013-03-26 14:25:00",
    //        "2013-03-28 14:07:27",
    //    }.Select(DateTime.Parse).ToList();

    //    // گروه‌بندی بر اساس روز
    //    var byDay = samples.GroupBy(d => d.Date)
    //                       .ToDictionary(g => g.Key, g => g.OrderBy(x => x).ToArray());

    //    var expected = new Dictionary<DateTime, int>
    //    {
    //        { DateTime.Parse("2013-01-14"), 0 },   // 21:00 → 0
    //        { DateTime.Parse("2013-01-15"), 0 },   // 21:00 → 0
    //        { DateTime.Parse("2013-02-07"), 21 },  // 06:23(8) + 15:27(13) = 21
    //        { DateTime.Parse("2013-02-08"), 60 },  // با سقف روزانه کَپ می‌شود
    //        { DateTime.Parse("2013-03-26"), 8  },  // 14:25 → 8
    //        { DateTime.Parse("2013-03-28"), 0  },  // قبل تعطیلات → 0
    //    };

    //    foreach (var kv in expected)
    //    {
    //        var day = kv.Key;
    //        var tax = Calc.GetDailyTax(Car(), byDay[day]);
    //        Assert.Equal(kv.Value, tax);
    //    }
    //}

    //// ——————————————————————
    //// Helper: دسترسی به GetTollFee از طریق RuleTable (هم‌ارزِ منطقی)
    //private int Private_GetFee(DateTime dt)
    //{
    //    var t = dt.TimeOfDay;
    //    foreach (var r in Rules.RateTable)
    //        if (r.Range.Contains(t)) return r.AmountSek;
    //    return 0;
    //}
}
