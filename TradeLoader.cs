using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace InvestmentBuilder
{
    [XmlType("stock")]
    public class Stock
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("symbol")]
        public string Symbol;
        [XmlElement("currency")]
        public string Currency;
        [XmlElement("number")]
        public int Number;
        [XmlElement("totalcost")]
        public double TotalCost;
        [XmlElement("scaling")]
        public double ScalingFactor;
    }

    [XmlRoot(ElementName="trades")]
    public class Trades
    {
        [XmlArray("buy")]
        public Stock[] Buys;
        [XmlArray("sell")]
        public Stock[] Sells;
        [XmlArray("changed")]
        public Stock[] Changed;
    }

    //loads the trades xml file data 
    static class TradeLoader
    {
        static public Trades GetTrades(string tradefile)
        {
            using (var fs = new FileStream(tradefile, FileMode.Open))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(Trades));
                return (Trades)serialiser.Deserialize(fs);
            }
        }
    }
}
