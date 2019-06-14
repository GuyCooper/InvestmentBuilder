using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using NLog;

namespace UserManagementService
{
    /// <summary>
    /// Interface represents a user notifier to call when a password change request has been made
    /// </summary>
    public interface IUserNotifier
    {
        void NotifyUser(string userAddress, string linkUrl);
    }

    /// <summary>
    /// Smtp notifer class. Allows password change notifications to be sent to a users email address
    /// </summary>
    public class SmtpNotifier : IUserNotifier
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public SmtpNotifier(string smtpServer, string from, string smtpUsername, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _from = from;
            _smtpPassword = smtpPassword;
            _smtpUsername = smtpUsername;
        }

        /// <summary>
        /// Email the user the password change link
        /// </summary>
        public void NotifyUser(string userAddress, string linkUrl)
        {
            try
            {
                SmtpClient mail = new SmtpClient(_smtpServer);
                mail.UseDefaultCredentials = false;
                mail.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                var message = File.ReadAllText("Template.html").Replace("<LINK>", linkUrl);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_from);
                mailMessage.To.Add(userAddress);
                mailMessage.Body = message;
                mailMessage.Subject = "Investment Builder Password Reset";
                mail.Send(mailMessage);
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error sending email notification");
            }
        }

        #endregion

        #region Private Data Members

        private readonly string _smtpServer;
        private readonly string _from;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }

    /// <summary>
    /// Test UserNotifier class.
    /// </summary>
    internal class TestNotifier : IUserNotifier
    {
        /// <summary>
        /// Notifer user of password change request.
        /// </summary>
        public void NotifyUser(string userAddress, string linkUrl)
        {
            Console.WriteLine($"user: {userAddress}. link: {linkUrl}");
            File.WriteAllText("testEmail.txt", $"link: {linkUrl}");
        }
    }
}
