using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ModelUNRegister.Startup))]
namespace ModelUNRegister
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
