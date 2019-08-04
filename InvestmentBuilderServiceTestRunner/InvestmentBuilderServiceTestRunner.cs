using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Run tests against the investmentbuilderservice process.
    /// </summary>
    class InvestmentBuilderServiceTestRunner
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        static void Main(string[] args)
        {
            var testRunner = new TestRunner(new Tests());
            testRunner.Initialise(false);
            testRunner.Run();
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
