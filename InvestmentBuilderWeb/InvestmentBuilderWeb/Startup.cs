using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InvestmentBuilderWeb.Startup))]
namespace InvestmentBuilderWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
