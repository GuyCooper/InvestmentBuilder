using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    [TestClass]
    public class BrokerManagerTests
    {
        [TestInitialize]
        public void Setup()
        {
            ContainerManager.RegisterType(typeof(InvestmentBuilder.BrokerManager), typeof(InvestmentBuilder.BrokerManager), false);
        }

        [TestMethod]
        public void When_Getting_All_Brokers()
        {
            var brokers = ContainerManager.ResolveValue<InvestmentBuilder.BrokerManager>().GetBrokers().ToList();
            Assert.AreEqual(3, brokers.Count);
        }

    }
}
