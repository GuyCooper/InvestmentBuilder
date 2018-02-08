using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace InvestmentBuilderService.Utils
{
    public interface IConnection
    {
        string ServerName { get; }
        string Username { get; }
        string Password { get; }
    }

    public interface IConnectionSettings
    {
        IConnection ServerConnection { get; }
        IConnection AuthServerConnection { get; }
    }

    [XmlType("connection")]
    public class Connection : IConnection
    {
        [XmlElement("server")]
        public string ServerName { get; set; }
        [XmlElement("username")]
        public string Username { get; set; }
        [XmlElement("password")]
        public string Password { get; set; }
    }

    [XmlRoot(ElementName = "connections")]
    public class ConnectionSettingsImpl
    {
        [XmlElement("serverConnection")]
        public Connection ServerConnection { get; set; }
        [XmlElement("authServerConnection")]
        public Connection AuthServerConnection { get; set; }
    }

    public class ConnectionSettings : IConnectionSettings
    {
        ConnectionSettingsImpl _settings;

        public ConnectionSettings(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(ConnectionSettingsImpl));
                _settings = (ConnectionSettingsImpl)serialiser.Deserialize(fs);
            }
        }

        [XmlElement("ServerConnection")]
        public IConnection ServerConnection { get { return _settings.ServerConnection; } }
        [XmlElement("AuthServerConnection")]
        public IConnection AuthServerConnection { get { return _settings.AuthServerConnection; } }
    }
}
