using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderWeb.Interfaces
{
    public interface IApplicationSessionService
    {
        void StartSession(string sessionId);
        void EndSession(string sessionId);
    }
}
