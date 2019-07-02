using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using InvestmentBuilderCore;
using InvestmentBuilderCore.Schedule;

namespace MarketDataServices
{
    /// <summary>
    /// MEF implememtation of IMarketSourceLocator.MEF is a service locator that can instantiate
    /// instances of types froma list of assemblies.
    /// </summary>
    internal class MefMarketSourceLocator : IMarketSourceLocator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MefMarketSourceLocator(IConfigurationSettings settings, ScheduledTaskFactory scheduledTaskFactory)
        {
            var catalog = new AggregateCatalog();
            //inject all IMarketDataSource instances in this assemlby
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMarketDataSource).Assembly));

            //ContainerManager.GetContainer().RegisterCatalog(catalog);

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            //now inject the datareader into all the different datasources
            foreach (IMarketDataSource source in Sources)
            {
                source.Initialise(settings, scheduledTaskFactory);
            }
        }

        #region IMarketSourceLocator

        /// <summary>
        /// Returns a list of available market data sources.
        /// </summary>
        [ImportMany(typeof(IMarketDataSource))]
        public IEnumerable<IMarketDataSource> Sources
        {
            get; private set;
        }

        #endregion

        #region Private Data

        private CompositionContainer _container;

        #endregion
    }
}
