using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace InvestmentBuilderCore
{
    public interface IConfigurationSettings
    {
        string DatasourceString { get; }
        string OutputFolder { get; }
        bool UpdateDatasource(string dataSource);
        bool UpdateOutputFolder(string folder);
        bool UpdateComparisonIndexes(IEnumerable<Index> indexes);
        string GetTradeFile(string account);
        string GetOutputPath(string account);
        string GetTemplatePath();
        IEnumerable<Index> ComparisonIndexes { get; }
    }

    [XmlType("index")]
    public class Index
    {
        [XmlElement("name")]
        public string Name{get;set;}
        [XmlElement("symbol")]
        public string Symbol {get;set;}
    }

    [XmlRoot(ElementName="configuration")]
    public class Configuration
    {
        [XmlElement("dataSource")]
        public string DatasourceString {get;set;}
        [XmlElement("outputFolder")]
        public string OutputFolder {get;set;}
        [XmlArray("indexes")]
        public Index[] IndexArray{get;set;}
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

        public string OutputFolder { get { return _configuration.OutputFolder; } }

        public IEnumerable<Index> ComparisonIndexes { get { return _configuration.IndexArray; } }

        public bool UpdateDatasource(string dataSource)
        {
            if(dataSource != _configuration.DatasourceString)
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
            return Path.Combine(_configuration.OutputFolder, "templates");
        }
    }
}
