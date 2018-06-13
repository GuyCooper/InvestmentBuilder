using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System;

namespace InvestmentBuilderCore
{
    /// <summary>
    /// Interface for all configuration settings in InvestmentBuilder
    /// </summary>
    public interface IConfigurationSettings
    {
        /// <summary>
        /// Datasource connection string (SQL Server connection)
        /// </summary>
        string DatasourceString { get; }
        /// <summary>
        /// Datasource connection string for authentication database (can be same as above)
        /// </summary>
        string AuthDatasourceString { get; }
        /// <summary>
        /// Output folder where all reports are written
        /// </summary>
        string OutputFolder { get; }
        /// <summary>
        /// url link to output folder
        /// </summary>
        string OutputLinkFolder { get; }
        /// <summary>
        /// List of comparison indexes to use in report
        /// </summary>
        IEnumerable<Index> ComparisonIndexes { get; }
        /// <summary>
        /// LIst of report formats to build. (EXCEL,PDF)
        /// </summary>
        IEnumerable<string> ReportFormats { get; }
        /// <summary>
        /// Path to Market Datasource file. File should contain a list of all
        /// market data stored in MArketData* format
        /// </summary>
        string MarketDatasource { get; }
        /// <summary>
        /// Location of cached market data (deprecated)
        /// </summary>
        string OutputCachedMarketData { get; }
        /// <summary>
        /// Update datasource methnod
        /// </summary>
        bool UpdateDatasource(string dataSource);
        /// <summary>
        /// UPdate output folder method. Called from Desktop app
        /// </summary>
        bool UpdateOutputFolder(string folder);
        /// <summary>
        /// Update comparison indexes. Called from desktop app.
        /// </summary>
        bool UpdateComparisonIndexes(IEnumerable<Index> indexes);
        /// <summary>
        /// Return the trade file for specified account (deprecated)
        /// </summary>
        string GetTradeFile(string account);
        /// <summary>
        /// return full path to report location for specified account. Also creates folder
        /// if it does not already exist
        /// </summary>
        string GetOutputPath(string account);
        /// <summary>
        /// return full path to url link to report folder. This is used by the web app.
        /// </summary>
        string GetOutputLinkPath(string account);
        /// <summary>
        /// Return path to template folder containing the excel templates.
        string GetTemplatePath();
    }

    [XmlType("index")]
    public class Index
    {
        [XmlElement("name")]
        public string Name{get;set;}
        [XmlElement("symbol")]
        public string Symbol {get;set;}
        [XmlElement("exchange")]
        public string Exchange { get; set; }
        [XmlElement("source")]
        public string Source { get; set; }
    }

    [XmlRoot(ElementName="configuration")]
    public class Configuration
    {
        [XmlElement("dataSource")]
        public string DatasourceString {get;set;}
        [XmlElement("authdataSource")]
        public string AuthDatasourceString { get; set; }
        [XmlElement("outputFolder")]
        public string OutputFolder {get;set;}
        [XmlElement("outputLinkFolder")]
        public string OutputLinkFolder { get; set; }
        [XmlArray("indexes")]
        public Index[] IndexArray{get;set;}
        [XmlArray("formats")]
        [XmlArrayItem("format")]
        public string[] ReportFormats { get; set; }
        [XmlElement("marketDataSource")]
        public string MarketDatasourceString { get; set; }
        [XmlElement("outputCachedMarketDataFile")]
        public string OutputCachedMarketData { get; set; }
    }

    public class ConfigurationSettings : IConfigurationSettings
    {
        private Configuration _configuration;

        public ConfigurationSettings(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(Configuration));
                _configuration = (Configuration)serialiser.Deserialize(fs);
            }
        }

        public string DatasourceString { get { return _configuration.DatasourceString; } }

        public string AuthDatasourceString { get { return _configuration.AuthDatasourceString; } }

        public string MarketDatasource { get { return _configuration.MarketDatasourceString; } }

        public string OutputCachedMarketData { get { return _configuration.OutputCachedMarketData; } }

        public string OutputFolder { get { return _configuration.OutputFolder; } }

        public string OutputLinkFolder { get { return _configuration.OutputLinkFolder; } }

        public IEnumerable<Index> ComparisonIndexes { get { return _configuration.IndexArray; } }

        public bool UpdateDatasource(string dataSource)
        {
            if (dataSource != _configuration.DatasourceString)
            {
                _configuration.DatasourceString = dataSource;
                return true;
            }
            return false;
        }

        public bool UpdateOutputFolder(string folder)
        {
            if (_configuration.OutputFolder != folder)
            {
                _configuration.OutputFolder = folder;
                return true;
            }
            return false;
        }

        public bool UpdateComparisonIndexes(IEnumerable<Index> indexes)
        {
            _configuration.IndexArray = indexes.ToArray();
            return true;
        }

        public string GetTradeFile(string account)
        {
            return Path.Combine(_configuration.OutputFolder, account, "Trades.xml");
        }

        public string GetOutputPath(string account)
        {
            var path = Path.Combine(_configuration.OutputFolder, account);
            Directory.CreateDirectory(path);
            return path;
        }

        public string GetOutputLinkPath(string account)
        {
            return $"{_configuration.OutputLinkFolder}/{account}";
        }

        public string GetTemplatePath()
        {
            return Path.Combine(_configuration.OutputFolder, "templates");
        }

        public IEnumerable<string> ReportFormats { get { return _configuration.ReportFormats; } }

    }
}
