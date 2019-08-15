using System.Xml;
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
        /// Datasource for authentication database
        /// </summary>
        string AuthenticationDatabase { get;}
        /// <summary>
        /// url and port the service will listen on
        /// </summary>
        string ListenURL { get; }
        /// <summary>
        /// url of web host
        /// </summary>
        string HostURL { get; }
        /// <summary>
        /// change password page. 
        /// </summary>
        string ChangePasswordPage { get; }
        /// <summary>
        /// validate new user page
        /// </summary>
        string ValidateNewUserPage { get; }

        /// <summary>
        /// max number of concurrent connections service will support
        /// </summary>
        int MaxConnections { get; }
        /// <summary>
        /// Web Server Root location
        /// </summary>
        string Root { get; }
        /// <summary>
        /// Smpt server name
        /// </summary>
        string SmtpServer { get; }
        /// <summary>
        /// Smtp username
        /// </summary>
        string SmtpUserName { get; }
        string SmtpPassword { get; }
        string OurEmailAddress { get; }
    }

    /// <summary>
    /// XML configuration class
    /// </summary>
    [XmlRoot(ElementName = "configuration")]
    public class ConfigurationImpl
    {
        [XmlElement("authdatabase")]
        public string AuthenticationDatabase { get; set; }

        [XmlElement("listenurl")]
        public string ListenURL { get; set; }

        [XmlElement("hosturl")]
        public string HostURL { get; set; }

        [XmlElement("changepasswordpage")]
        public string ChangePasswordPage { get; set; }

        [XmlElement("validatenewuserpage")]
        public string ValidateNewUserPage { get; set; }

        [XmlElement("maxconnections")]
        public int MaxConnections { get; set; }

        [XmlElement("root")]
        public string Root { get; set; }

        [XmlElement("smtpserver")]
        public string SmtpServer { get; set; }

        [XmlElement("smtpuser")]
        public string SmtpUserName { get; set; }

        [XmlElement("smtppassword")]
        public string SmtpPassword { get; set; }

        [XmlElement("address")]
        public string OurEmailAddress { get; set; }
    }

    /// <summary>
    /// Configuration class. contains XML configuration class
    /// </summary>
    internal class Configuration : IConfiguration
    {
        #region IConfiguration

        public string AuthenticationDatabase { get { return _configuration.AuthenticationDatabase; } }

        public string ListenURL { get { return _configuration.ListenURL; } }

        public string HostURL { get { return _configuration.HostURL; } }

        public string ChangePasswordPage { get { return _configuration.ChangePasswordPage; } }

        public string ValidateNewUserPage { get { return _configuration.ValidateNewUserPage; } }

        public int MaxConnections { get { return _configuration.MaxConnections; } }

        public string Root { get { return _configuration.Root; } }

        public string SmtpServer { get { return _configuration.SmtpServer; } }

        public string SmtpUserName { get { return _configuration.SmtpUserName; } }

        public string SmtpPassword { get { return _configuration.SmtpPassword; } }

        public string OurEmailAddress { get { return _configuration.OurEmailAddress; } }

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
