using System.IO;
using System.Xml.Serialization;

namespace InvestmentBuilderFileHandler
{
    /// <summary>
    /// Configuration for the InvestmentBuilder file handler.
    /// </summary>
    [XmlRoot(ElementName = "configuration")]
    public class InvestmentBuilderFileHandlerConfig
    {
        #region Public Properties

        /// <summary>
        /// Location of valuation reports.
        /// </summary>
        [XmlElement("reportFolder")]
        public string ReportFolder { get; set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public static InvestmentBuilderFileHandlerConfig CreateConfig(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(InvestmentBuilderFileHandlerConfig));
                return (InvestmentBuilderFileHandlerConfig)serialiser.Deserialize(fs);
            }
        }

        #endregion
    }
}
