using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.IO;

namespace InvestmentBuilderClient
{
    class ConfigurationSettings
    {
        public ConfigurationSettings()
        {
            _LoadValues();
            //TradeFile = System.Configuration.ConfigurationSettings.AppSettings["tradeFile"] ?? string.Empty;
            //DatasourceString = System.Configuration.ConfigurationSettings.AppSettings["dataSource"] ?? string.Empty;
            //OutputFolder = System.Configuration.ConfigurationSettings.AppSettings["outputFolder"] ?? string.Empty;
        }

        private void _LoadValues()
        {
            using (var reader = new StreamReader("InvestmentBuilderConfig.xml"))
            {
                var parser = XmlTextReader.Create(reader);
                while(parser.Read())
                {
                    if (parser.Name == "investmentBuilder")
                    {
                        TradeFile = parser.GetAttribute("tradeFile");
                        DatasourceString = parser.GetAttribute("dataSource");
                        OutputFolder = parser.GetAttribute("outputFolder");
                        break;
                    }
                }
                parser.Close();
            }
            
        }

        public string TradeFile {get; private set;}

        public string DatasourceString { get; private set; }

        public string OutputFolder { get; private set; }

        public bool UpdateTradeFile(string tradeFile)
        {
            if(tradeFile != TradeFile)
            {
                TradeFile = tradeFile;
                return true;
            }
            return false;
        }

        public bool UpdateDatasource(string dataSource)
        {
            if(dataSource != DatasourceString)
            {
                DatasourceString = dataSource;
                return true;
            }
            return false;
        }

        public bool UpdateOutputFolder(string folder)
        {
            if(OutputFolder != folder)
            {
                OutputFolder = folder;
                return true;
            }
            return false;
        }
    }
}
