using Microsoft.AspNetCore.Builder;

namespace P7.Core.Startup
{
    public abstract class ConfigureRegistrant : IConfigureRegistrant
    {

        public abstract void OnConfigure(IApplicationBuilder app);
    }
}