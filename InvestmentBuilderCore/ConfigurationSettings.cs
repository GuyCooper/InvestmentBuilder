using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using InvestmentBuilderCore.Schedule;
using System;
using NLog;
using System.Reflection;

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

        /// <summary>
        /// Path to audit file. Logs all input and output message
        /// </summary>
        string AuditFileName { get; }

        /// <summary>
        /// Is Using Test Configuration
        /// </summary>
        bool IsTest { get; }

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

        [XmlElement("testdataSource")]
        public string TestDatasourceString { get; set; }

        [XmlElement("authdataSource")]
        public string AuthDatasourceString { get; set; }
        
        [XmlElement("outputFolder")]
        public string OutputFolder {get;set;}

        [XmlElement("testoutputFolder")]
        public string TestOutputFolder { get; set; }

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
        
        [XmlElement("auditFile")]
        public string AuditFileName { get; set; }
        
        [XmlArray("schedule")]
        public ScheduledTaskDetails[] ScheduledTasks { get; set; }
    }

    /// <summary>
    /// XML Implementation of IConfigurationSettings
    /// </summary>
    public class ConfigurationSettings : IConfigurationSettings
    {
        #region Public Properties

        public string DatasourceString { get { return m_test ? m_configuration.TestDatasourceString : m_configuration.DatasourceString; } }
         
        public string AuthDatasourceString { get { return m_configuration.AuthDatasourceString; } }

        public string MarketDatasource { get { return m_configuration.MarketDatasourceString; } }

        public string OutputCachedMarketData { get { return m_configuration.OutputCachedMarketData; } }

        public int MaxAccountsPerUser { get { return m_configuration.MaxAccountsPerUser; } }

        public string OutputFolder { get { return GetOutputfolder(); } }

        public IEnumerable<Index> ComparisonIndexes { get { return m_configuration.IndexArray; } }

        public string ScriptFolder { get { return m_configuration.ScriptFolder; } }

        public IEnumerable<string> ReportFormats { get { return m_configuration.ReportFormats; } }

        /// <summary>
        /// List of scheduled tasks.
        /// </summary>
        public IEnumerable<ScheduledTaskDetails> ScheduledTasks { get { return m_configuration.ScheduledTasks; } }

        /// <summary>
        /// Audit file name.
        /// </summary>
        public string AuditFileName { get { return m_configuration.AuditFileName; } }

        public bool IsTest {  get { return m_test; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor - just takes filename
        /// </summary>
        public ConfigurationSettings(string filename) : this(filename, new List<Tuple<string, string>>(), null, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfigurationSettings(string filename, List<Tuple<string,string>> overrides, string certificate, bool test)
        {
            m_configuration = XmlConfigFileLoader.LoadConfiguration<Configuration>(filename, certificate);

            logger.Info("Configuration:");
            logger.Info(File.ReadAllText(filename));

            m_test = test;

            //now apply the overrides
            var props = m_configuration.GetType().
                GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).ToList();

            foreach(var ovride in overrides)
            {
                var propinfo = props.FirstOrDefault(p => MatchPropertyInfoXmlName(p, ovride.Item1));
                if(propinfo != null)
                {
                    logger.Info($"Override configuration. setting {propinfo.Name} to {ovride.Item2}");

                    if(propinfo.PropertyType == typeof(int))
                    {
                        propinfo.SetValue(m_configuration, Convert.ToInt32(ovride.Item2));
                    }
                    else if (propinfo.PropertyType == typeof(double))
                    {
                        propinfo.SetValue(m_configuration, Convert.ToDouble(ovride.Item2));
                    }
                    else if (propinfo.PropertyType == typeof(string))
                    {
                        propinfo.SetValue(m_configuration, ovride.Item2);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// update the datasource
        /// </summary>
        public bool UpdateDatasource(string dataSource)
        {
            if (dataSource != m_configuration.DatasourceString)
            {
                m_configuration.DatasourceString = dataSource;
                return true;
            }
            return false;
        }

        public bool UpdateOutputFolder(string folder)
        {
            if (m_configuration.OutputFolder != folder)
            {
                m_configuration.OutputFolder = folder;
                return true;
            }
            return false;
        }

        public bool UpdateComparisonIndexes(IEnumerable<Index> indexes)
        {
            m_configuration.IndexArray = indexes.ToArray();
            return true;
        }

        public string GetTradeFile(string account)
        {
            return Path.Combine(GetOutputfolder(), account, "Trades.xml");
        }

        public string GetOutputPath(string account)
        {
            var path = Path.Combine(GetOutputfolder(), account);
            Directory.CreateDirectory(path);
            return path;
        }

        public string GetTemplatePath()
        {
            return m_configuration.TemplatePath;
        }

        #endregion

        #region Private Methods

        private string GetOutputfolder()
        {
            return m_test ? m_configuration.TestOutputFolder : m_configuration.OutputFolder;
        }

        /// <summary>
        /// Method matches the xmlattribute of the propertyinfo to the supplied name.
        /// </summary>
        private bool MatchPropertyInfoXmlName(PropertyInfo propInfo, string name)
        {
            var xmlAttr = propInfo.CustomAttributes.FirstOrDefault(p => p.AttributeType.Name == "XmlElementAttribute");
            if(xmlAttr != null)
            {
                var arg = xmlAttr.ConstructorArguments.FirstOrDefault(c => string.Equals(c.Value.ToString(),name,StringComparison.CurrentCultureIgnoreCase));
                return arg != null;
            }
            return false;
        }

        #endregion

        #region Private Data

        private Configuration m_configuration;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool m_test;

        #endregion

    }
}
