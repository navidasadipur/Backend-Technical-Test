using System;

namespace congestion.calculator
{
    internal interface IGetsFeeByTime
    {
        decimal GetFeeByTime(TimeOnly timeOnly);
    }
}


