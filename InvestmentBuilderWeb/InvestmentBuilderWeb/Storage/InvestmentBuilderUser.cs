using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using InvestmentBuilderWeb.Models;

namespace InvestmentBuilderWeb.Storage
{
    public class InvestmentBuilderUserStore : IUserStore<ApplicationUser>
    {
        private List<ApplicationUser> _users;

        public Task CreateAsync(ApplicationUser user)
        {
            return new Task(() => _users.Add(user));
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return new Task(() =>
            {
                int index = _users.FindIndex(x => string.Equals(x.Id, user.Id));
                if (index > -1)
                {
                    _users.RemoveAt(index);
                }
            });
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return new Task<ApplicationUser>(() =>
            {
                return _users.FirstOrDefault(x => string.Equals(x.Id, userId));
            });
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return new Task<ApplicationUser>(() =>
            {
                return _users.FirstOrDefault(x => string.Equals(x.UserName, userName));
            });
 
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return new Task(() =>
            {
                var updateUser = _users.FirstOrDefault(x => string.Equals(x.Id, user.Id));
                if(updateUser != null)
                {
                    updateUser.UserName = user.UserName;
                }
            });  
        }

        public void Dispose()
        {
        }
    }
}