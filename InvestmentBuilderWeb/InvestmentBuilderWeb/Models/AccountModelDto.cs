using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    public class AccountModelDto
    {
        public AccountModelDto()
        {
            Members = new List<AccountMemberDto>();
            AccountEnabled = true;
        }

        [Required(ErrorMessage = "You must enter a Name")]
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string AccountPassword { get; set; }
        [Required(ErrorMessage = "You must enter a valid reporting currency")]
        public string ReportingCurrency { get; set; }
        [Required(ErrorMessage = "You must enter a valid type")]
        public string AccountType { get; set; }
        public bool AccountEnabled { get; set; }
        public string Broker { get; set; }
        public string SerialisedMembers { get; set; }

        public IList<AccountMemberDto> Members { get; set; }
    }

    public class AccountMemberDto
    {
        public string MemberID { get; set; }
        public string AuthorisationType { get; set; }
    }
}