using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    class PortfolioResponseDto : Dto
    {
        public IEnumerable<CompanyData> Portfolio { get; private set; }

        public PortfolioResponseDto(IEnumerable<CompanyData> portfolio)
        {
            Portfolio = portfolio;
        }
    }

    /// <summary>
    /// handler class for retreiving the portfolio
    /// </summary>
    class GetPortfolioChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        private InvestmentBuilder.InvestmentBuilder _builder;
        public GetPortfolioChannel(AccountService accountService, InvestmentBuilder.InvestmentBuilder builder) :
            base("GET_PORTFOLIO_REQUEST", "GET_PORTFOLIO_RESPONSE", accountService)
        {
            _builder = builder;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater update)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new PortfolioResponseDto(_builder.GetCurrentInvestments(userToken, userSession.UserPrices).OrderBy(x => x.Name).ToList());
        }
    }
}
