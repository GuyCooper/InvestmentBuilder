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

    /// <summary>
    /// handler class for updating valuation date command
    /// </summary>
    class UpdateValuationDateChannel : EndpointChannel<UpdateValuationDateDto, ChannelUpdater>
    {
        public UpdateValuationDateChannel(AccountService accountService) 
            : base("UPDATE_VALUTION_DATE_REQUEST", "UPDATE_VALUATION_DATE_RESPONSE", accountService)
        {
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, UpdateValuationDateDto payload, ChannelUpdater update)
        {
            userSession.ValuationDate = payload.ValautionDate;
            return new ResponseDto { Status = true };
        }
    }
}
