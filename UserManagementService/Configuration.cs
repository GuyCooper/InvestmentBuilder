﻿using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

namespace UserManagementService
{
    /// <summary>
    /// Configuration interface
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Datasource for application datbase
        /// </summary>
        string ApplicationDatabase { get; }
        /// <summary>
        /// Datasource for authentication database
        /// </summary>
        string AuthenticationDatabase { get;}
        /// <summary>
        /// url and port the service will listen on
        /// </summary>
        string ListenURL { get; }
        /// <summary>
        /// max number of concurrent connections service will support
        /// </summary>
        int MaxConnections { get; }
        /// <summary>
        /// Web Server Root location
        /// </summary>
        string Root { get; }
    }

    /// <summary>
    /// XML configuration class
    /// </summary>
    [XmlRoot(ElementName = "configuration")]
    public class ConfigurationImpl
    {
        [XmlElement("appdatabase")]
        public string ApplicationDatabase { get; set; }
        [XmlElement("authdatabase")]
        public string AuthenticationDatabase { get; set; }

        [XmlElement("listenurl")]
        public string ListenURL { get; set; }

        [XmlElement("maxconnections")]
        public int MaxConnections { get; set; }

        [XmlElement("root")]
        public string Root { get; set; }
    }

    /// <summary>
    /// Configuration class. contains XML configuration class
    /// </summary>
    internal class Configuration : IConfiguration
    {
        #region IConfiguration

        public string ApplicationDatabase { get { return _configuration.ApplicationDatabase; } }

        public string AuthenticationDatabase { get { return _configuration.AuthenticationDatabase; } }

        public string ListenURL { get { return _configuration.ListenURL; } }

        public int MaxConnections { get { return _configuration.MaxConnections; } }

        public string Root { get { return _configuration.Root; } }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor. deserialise configuration from file
        /// </summary>
        public Configuration(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(ConfigurationImpl));
                _configuration = (ConfigurationImpl)serialiser.Deserialize(fs);
            }
        }

        #endregion

        #region Private Members

        private readonly ConfigurationImpl _configuration;

        #endregion
    }
}
