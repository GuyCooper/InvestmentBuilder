using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Interfaces;

namespace InvestmentBuilderWeb.Utils
{
    internal class BuildReportMonitor : IProgressCounter, IBuildMonitor
    {
        private int count_;
        private int increment_;
        private string username_;
        private bool isbuilding_;
        private IEnumerable<string> errors_;

        private static Dictionary<string, List<string>> ErrorLookup = new Dictionary<string, List<string>>();

        public BuildReportMonitor(string username)
        {
            count_ = 0;
            increment_ = 0;
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

        public void IncrementCounter()
        {
            count_ += increment_;
        }

        public void ResetCounter(int Increment)
        {
            increment_ = Increment;
            count_ = 0;
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
                Progress = count_,
                IsBuilding = isbuilding_,
                Errors = errors_
            };
        }

        public IProgressCounter GetProgressCounter()
        {
            return this;
        }
    }
}