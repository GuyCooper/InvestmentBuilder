using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.Contracts;

namespace InvestmentBuilder
{
    interface IBroker
    {
        string Name { get; }
        double GetNetSellingValue(double quantity, double price);
    }

    internal abstract class BrokerContract : IBroker
    {
        public string Name
        {
            get
            {
                Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == false);
                return null;
            }
        }
        

        public double GetNetSellingValue(double quantity, double price)
        {
            Contract.Requires(quantity > 0);
            Contract.Requires(price > 0);
            return 0;
        }
    }

    public class BrokerManager
    {
        [ImportMany(typeof(IBroker))]
        private IEnumerable<IBroker> Brokers { get; set; }

        private CompositionContainer _container;

        public BrokerManager()
        {
            var catalog = new AggregateCatalog();
            //inject all IMarketDataSource instances in this assemlby
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IBroker).Assembly));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }

        public double GetNetSellingValue(string broker, double quantity, double price)
        {
            if (string.IsNullOrEmpty(broker) == false)
            {
                var result = Brokers.FirstOrDefault(x => x.Name == broker);
                if (result != null)
                {
                    return result.GetNetSellingValue(quantity, price);
                }
            }
            //if broker not specified or found just return default of value  of gross value 
            return quantity * price;
        }

        public IEnumerable<string> GetBrokers()
        {
            return Brokers.Select(x => x.Name);
        }
    }

    [Export(typeof(IBroker))]
    class ShareCentreBroker : IBroker
    {
        public string Name
        {
            get { return "ShareCentre"; }
        }

        public double GetNetSellingValue(double quantity, double price)
        {
            double dGrossValue = quantity * price;
            if (dGrossValue > 750d)
                return dGrossValue - (dGrossValue * 0.01);
            return dGrossValue - 7.5d;
        }
    }

    [Export(typeof(IBroker))]
    class HargreavesLansdownBroker : IBroker
    {
        public string Name
        {
            get { return "HargreavesLansdown"; }
        }

        public double GetNetSellingValue(double quantity, double price)
        {
            double dGrossValue = quantity * price;
            return dGrossValue - 12.95d;
        }
    }

    [Export(typeof(IBroker))]
    class AJBellBroker : IBroker
    {
        public string Name
        {
            get { return "AJBell"; }
        }

        public double GetNetSellingValue(double quantity, double price)
        {
            double dGrossValue = quantity * price;
            return dGrossValue - 8.95d;
        }
    }

}
