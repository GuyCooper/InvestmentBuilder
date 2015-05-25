﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    /// <summary>
    /// aggregates all market data sources and iterates through each one to get 
    /// source data until succeeds
    /// </summary>
    public class AggregatedMarketDataSource : IMarketDataSource
    {
        [ImportMany(typeof(IMarketDataSource))]
        private IEnumerable<IMarketDataSource> Sources { get; set; }

        private CompositionContainer _container;

        public string Name { get { return "Aggregated"; } }

        public AggregatedMarketDataSource()
        {
            var catalog = new AggregateCatalog();
            //inject all IMarketDataSource instances in this assemlby
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMarketDataSource).Assembly));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }

        public bool TryGetMarketData(string symbol, string exchange, out double dData)
        {
            foreach(var source in Sources)
            {
                if(source.TryGetMarketData(symbol, exchange, out dData))
                {
                    return true;
                }
            }

            dData = 0d;
            return false;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            foreach (var source in Sources)
            {
                if (source.TryGetFxRate(baseCurrency, contraCurrency, out dFxRate))
                {
                    return true;
                }
            }

            dFxRate = 0d;
            return false;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            foreach (var source in Sources)
            {
                var result = source.GetHistoricalData(instrument, dtFrom);
                if(result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
