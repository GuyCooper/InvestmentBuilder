using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using InvestmentBuilderCore.Schedule;

namespace InvestmentBuilderCore
{
    /// <summary>
    /// Interface for all configuration settings in InvestmentBuilder
    /// </summary>
    public interface IConfigurationSettings
    {
        #region Public Properties

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
        /// Maximum number of accounts each user is allowed to be a member of
        /// </summary>
        int MaxAccountsPerUser { get; }
        /// <summary>
        /// Folder containing any external scriopts to be run.
        /// </summary>
        string ScriptFolder { get; }
        
        /// <summary>
        /// Lst of scheduled tasks.
        /// </summary>
        IEnumerable<ScheduledTaskDetails> ScheduledTasks { get; }

        #endregion

        #region Public Methods

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
        /// Return path to template folder containing the excel templates.
        string GetTemplatePath();

        #endregion
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
        [XmlArray("indexes")]
        public Index[] IndexArray{get;set;}
        [XmlArray("formats")]
        [XmlArrayItem("format")]
        public string[] ReportFormats { get; set; }
        [XmlElement("marketDataSource")]
        public string MarketDatasourceString { get; set; }
        [XmlElement("outputCachedMarketDataFile")]
        public string OutputCachedMarketData { get; set; }
        [XmlElement("maxAccountsPerUser")]
        public int MaxAccountsPerUser { get; set; }
        [XmlElement("templatePath")]
        public string TemplatePath { get; set; }
        [XmlElement("scriptFolder")]
        public string ScriptFolder { get; set; }
        [XmlArray("schedule")]
        public ScheduledTaskDetails[] ScheduledTasks { get; set; }

    }

    /// <summary>
    /// XML Implementation of IConfigurationSettings
    /// </summary>
    public class ConfigurationSettings : IConfigurationSettings
    {
        #region Public Properties

        public string DatasourceString { get { return _configuration.DatasourceString; } }
         
        public string AuthDatasourceString { get { return _configuration.AuthDatasourceString; } }

        public string MarketDatasource { get { return _configuration.MarketDatasourceString; } }

        public string OutputCachedMarketData { get { return _configuration.OutputCachedMarketData; } }

        public int MaxAccountsPerUser { get { return _configuration.MaxAccountsPerUser; } }

        public string OutputFolder { get { return _configuration.OutputFolder; } }

        public IEnumerable<Index> ComparisonIndexes { get { return _configuration.IndexArray; } }

        public string ScriptFolder { get { return _configuration.ScriptFolder; } }

        public IEnumerable<string> ReportFormats { get { return _configuration.ReportFormats; } }

        /// <summary>
        /// List of scheduled tasks.
        /// </summary>
        public IEnumerable<ScheduledTaskDetails> ScheduledTasks { get { return _configuration.ScheduledTasks; } }

        #endregion

        #region Constructor

        public ConfigurationSettings(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(Configuration));
                _configuration = (Configuration)serialiser.Deserialize(fs);
            }
        }

        #endregion

        #region Public Methods

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

        public string GetTemplatePath()
        {
            return _configuration.TemplatePath;
        }

        #endregion

        #region Private Data

        private Configuration _configuration;

        #endregion

    }
}
