using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    internal class MefMarketSourceLocator : IMarketSourceLocator
    {
        private CompositionContainer _container;

        [ImportMany(typeof(IMarketDataSource))]
        public IEnumerable<IMarketDataSource> Sources
        {
            get; private set;
        }

        public MefMarketSourceLocator(IConfigurationSettings settings)
        {
            var catalog = new AggregateCatalog();
            //inject all IMarketDataSource instances in this assemlby
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMarketDataSource).Assembly));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            //now inject the datareader into all the different datasources
            foreach (IMarketDataSource source in Sources)
            {
                source.Initialise(settings);
            }
        }
    }
}
