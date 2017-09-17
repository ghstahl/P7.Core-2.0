using Microsoft.AspNetCore.Builder;

namespace P7.Core.Startup
{
    public interface IConfigureRegistrant
    {
        void OnConfigure(IApplicationBuilder app);
    }
}