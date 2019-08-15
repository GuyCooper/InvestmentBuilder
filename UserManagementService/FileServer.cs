using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace UserManagementService
{
    /// <summary>
    /// Class handles file requests from web server.
    /// </summary>
    class FileServer
    {
        #region Public Methods

        public FileServer(string root)
        {
            m_root = root;
            var files = Directory.EnumerateFiles(root??".", "*.*", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                var subFile = file.Replace(root, "").TrimStart('\\');
                m_fileList.Add(subFile);
            }
        }

        //handle an incoming web request. returns true if request was handled otherwise returns false
        public bool HandleRequest(string request, ref string strOut, Dictionary<string, List<string>> headers )
        {
            var file = m_fileList.Find(x => string.Equals(x, request, StringComparison.InvariantCultureIgnoreCase));
            if(file != null)
            {
                strOut = File.ReadAllText(Path.Combine(m_root, file));
                return true;        
            }
            return false;
        }

        #endregion

        #region Private Data

        private List<string> m_fileList = new List<string>();
        private readonly string m_root;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}
