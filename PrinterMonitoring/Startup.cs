using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PrinterMonitoring.Startup))]
namespace PrinterMonitoring
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
