using Autofac;

namespace P7.Identity
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityQueryInput>();

            //   builder.RegisterType<MyFieldRecordRegistration>().As<IFieldRecordRegistration>();
            //   builder.RegisterType<MyFieldRecordRegistration2>().As<IFieldRecordRegistration>();
        }
    }
}