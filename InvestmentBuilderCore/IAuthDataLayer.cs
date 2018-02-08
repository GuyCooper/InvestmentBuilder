using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public interface IAuthDataLayer
    {
        int AddNewUser(string userName, string eMail, string salt, string passwordHash, string phoneNumber, bool twoFactorEnabled);
        bool AuthenticateUser(string email, string passwordHash);
        bool ChangePassword(string email, string oldPasswordHash, string newPasswordHash, string newSalt);
        void RemoveUser(string email);
        string GetSalt(string email);
    }
}
