using Autofac;

namespace P7.HealthCheck.Core
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthCheckStore>()
                .As<IHealthCheckStore>()
                .SingleInstance();
            builder.RegisterType<AggregateHealthCheck>();
            //   builder.RegisterType<MyFieldRecordRegistration>().As<IFieldRecordRegistration>();
            //   builder.RegisterType<MyFieldRecordRegistration2>().As<IFieldRecordRegistration>();
        }
    }
}