using System;

namespace InvestmentBuilderCore
{
    public interface IClock
    {
        DateTime GetCurrentTime();
    }

    public class UtcClock : IClock
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }            
    }
}
