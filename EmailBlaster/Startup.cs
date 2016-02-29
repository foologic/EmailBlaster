using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmailBlaster.Startup))]
namespace EmailBlaster
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
