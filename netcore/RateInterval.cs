using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion.calculator
{
    public sealed class RateInterval
    {
        public TimeRange Range { get; }
        public int AmountSek { get; }
        public RateInterval(TimeRange range, int amountSek)
        {
            Range = range; AmountSek = amountSek;
        }
    }
}
