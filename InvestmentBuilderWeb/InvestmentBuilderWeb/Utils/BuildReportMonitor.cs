using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Interfaces;

namespace InvestmentBuilderWeb.Utils
{
    internal class BuildReportMonitor : IBuildMonitor
    {
        private string username_;
        private bool isbuilding_;
        private IEnumerable<string> errors_;
        private ProgressCounter counter_;

        private static Dictionary<string, List<string>> ErrorLookup = new Dictionary<string, List<string>>();

        public BuildReportMonitor(string username)
        {
            counter_ = new ProgressCounter();
            username_ = username;
            isbuilding_ = false;
        }

        public void StartBuilding()
        {
            isbuilding_ = true;
            errors_ = null;
        }

        public void StopBuiliding()
        {
            isbuilding_ = false;
            errors_ = _GetUserErrors();
        }

        public void AddError(string error)
        {
            if(errors_ == null)
            {
                errors_ = new List<string>
                {
                    error
                };
            }
        }
        public static void LogMethod(string level, string message)
        {
            //errors that are displayed in client. only include error message
            if (level.Equals("ERROR", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                int index = message.IndexOf(';');
                if (index > 0)
                {
                    string user = message.Substring(0, index);
                    string errorData = message.Substring(index + 1);

                    List<string> errorList;
                    if (ErrorLookup.TryGetValue(user, out errorList) == false)
                    {
                        errorList = new List<string>();
                        ErrorLookup.Add(user, errorList);
                    }
                    errorList.Add(errorData);
                }
            }
        }

        private IEnumerable<string> _GetUserErrors()
        {
            IEnumerable<string> result = null;
            if (ErrorLookup.ContainsKey(username_))
            {
                result = ErrorLookup[username_];
                ErrorLookup[username_].Clear();
            }
            return result;
        }

        public ReportStatus GetReportStatus()
        {
            return new ReportStatus
            {
                Progress = counter_.Count,
                BuildSection = counter_.Section,
                IsBuilding = isbuilding_,
                Errors = errors_
            };
        }

        public ProgressCounter GetProgressCounter()
        {
            return counter_;
        }
    }
}