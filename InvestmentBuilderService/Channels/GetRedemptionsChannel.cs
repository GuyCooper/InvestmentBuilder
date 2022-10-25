using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class RedemptionDto
    {
        public string User { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public double RedeemedUnits { get; set; }
        public string Status { get; set; }

    }

    /// <summary>
    /// Dto for returning the list of redemptions to a client.
    /// </summary>
    internal class RedemptionsDto : Dto
    {
        public List<RedemptionDto> Redemptions { get; set; }
    }

    /// <summary>
    /// Handles the GetRedemptions request.
    /// </summary>
    class GetRedemptionsChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public GetRedemptionsChannel(ServiceAggregator aggregator) : 
            base("GET_REDEMPTIONS_REQUEST", "GET_REDEMPTIONS_RESPONSE", aggregator)
        {
            _builder = aggregator.Builder;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Handle the GetRedemptions request and return the list of redemptions for this users account.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater updater)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new RedemptionsDto
            {
                Redemptions = _builder.GetRedemptions(userToken, userSession.ValuationDate).
                                       Where(redemption => redemption.Status != RedemptionStatus.Complete)
                                       .Select(r => new RedemptionDto
                                       {
                                           User = r.User,
                                           Amount = r.Amount,
                                           TransactionDate = r.TransactionDate,
                                           RedeemedUnits = r.RedeemedUnits,
                                           Status = r.Status.ToString()
                                       })
                                       .ToList()
            };
        }

        #endregion

        #region Private Data

        private readonly InvestmentBuilder.InvestmentBuilder _builder;

        #endregion
    }
}
