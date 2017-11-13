using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Module = Autofac.Module;

namespace OldSchool.RazorPages
{
    public class AutofacModule : Module
    {
        static ILogger logger = Log.ForContext<AutofacModule>();
        protected override void Load(ContainerBuilder builder)
        {
            logger.Information("Hi from OldSchool AutoFac Loader!");
            var assembly = this.GetType().GetTypeInfo().Assembly;
 
                
            builder.RegisterType<OldSchoolMagicService>()
                .As<OldSchoolMagicService>()
                .As<IOldSchoolMagicService>();
        }
    }
}
