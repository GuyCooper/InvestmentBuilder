using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    class TestProgressCounter : IProgressCounter
    {
        public int Count { get; private set; }

        private int _increment;
        public void IncrementCounter()
        {
            Count += _increment;
        }

        public void ResetCounter(int Increment)
        {
            _increment = Increment;
            Count = 0;
        }
    }
}
