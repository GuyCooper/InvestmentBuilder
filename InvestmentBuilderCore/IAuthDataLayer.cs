
namespace InvestmentBuilderCore
{
    /// <summary>
    /// IAuthDataLayer interface. class manages user authentication.
    /// </summary>
    public interface IAuthDataLayer
    {
        int AddNewUser(string userName, string eMail, string salt, string passwordHash, string phoneNumber, bool twoFactorEnabled);
        bool AuthenticateUser(string email, string passwordHash);
        bool ChangePassword(string email, string oldPasswordHash, string newPasswordHash, string newSalt);
        void RemoveUser(string email);
        string GetSalt(string email);
        bool ValidateUser(string emailAddress);
    }
}
