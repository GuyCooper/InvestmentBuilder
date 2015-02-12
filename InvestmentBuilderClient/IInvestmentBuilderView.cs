using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderClient
{
    interface IInvestmentBuilderView
    {
        void UpdateValuationDate(DateTime dtValuation);
        void CommitData();
    }
}
