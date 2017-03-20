using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public interface IProgressCounter
    {
        void ResetCounter(int Increment);
        void IncrementCounter();
    }
}