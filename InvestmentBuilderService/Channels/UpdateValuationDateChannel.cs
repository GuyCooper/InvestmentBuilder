using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class UpdateValuationDateDto : Dto
    {
        public DateTime ValautionDate { get; set; }
    }

    class UpdateValuationDateChannel : EndpointChannel<UpdateValuationDateDto>
    {
        public UpdateValuationDateChannel(AccountService accountService) 
            : base("UPDATE_VALUTION_DATE_REQUEST", "UPDATE_VALUATION_DATE_RESPONSE", accountService)
        {
        }

        public override Dto HandleEndpointRequest(UserSession userSession, UpdateValuationDateDto payload)
        {
            userSession.ValuationDate = payload.ValautionDate;
            return new ResponseDto { Status = true };
        }
    }
}
