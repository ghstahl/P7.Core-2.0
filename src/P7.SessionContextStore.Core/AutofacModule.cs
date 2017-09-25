using Autofac;

namespace P7.SessionContextStore.Core
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SessionContextStore>().As<ISessionContextStore>().SingleInstance();
            builder.RegisterType<SessionContext>().As<ISessionContext>();
        }
    }
}