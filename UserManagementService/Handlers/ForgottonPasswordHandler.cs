using System;
using System.Collections.Generic;
using InvestmentBuilderCore;
using System.Net.Mail;
using System.Net;

namespace UserManagementService.Handlers
{
    /// <summary>
    /// ForgottonPassword Request Dto
    /// </summary>
    class ForgottonPasswordRequest
    {
        public string EMailAddress { get; set; }
    }

    /// <summary>
    /// Forgotton password response dto
    /// </summary>
    class ForgottonPasswordResponse
    {

    }

    /// <summary>
    /// Class for handling a forgotton password request. Validates the email address and sends
    /// a new password to this address.
    /// </summary>
    class ForgottonPasswordHandler : GetRequestHandler<ForgottonPasswordRequest, ForgottonPasswordResponse>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ForgottonPasswordHandler(string smtpServer, string username, string password,
            string from, IAuthDataLayer authdata) :  base("ForgottonPassword")
        {
            _smtpServer = smtpServer;
            _username = username;
            _password = password;
            _from = from;
            _authdata = authdata;
        }

        /// <summary>
        /// Handle request.
        /// </summary>
        protected override ForgottonPasswordResponse ProcessRequest(ForgottonPasswordRequest request, Dictionary<string, List<string>> headers)
        {
            //validate email address.
            //send an smtp message to address with temporary password   

            var password = SaltedHash.GenerateSalt().Substring(0, 8);


            SmtpClient mail = new SmtpClient(_smtpServer);
            mail.UseDefaultCredentials = false;
            mail.Credentials = new NetworkCredential(_username, _password);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_from);
            mailMessage.To.Add(request.EMailAddress);
            mailMessage.Body = "body";
            mailMessage.Subject = "subject";
            mail.Send(mailMessage);

            return new ForgottonPasswordResponse();
        }

        #endregion

        #region Private Data Members

        private readonly IAuthDataLayer _authdata;
        private readonly string _smtpServer;
        private readonly string _username;
        private readonly string _password;
        private readonly string _from;

        #endregion

    }
}
