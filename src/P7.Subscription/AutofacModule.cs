using Autofac;

namespace P7.Subscription
{

    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubscriptionQueryInput>();
            builder.RegisterType<MetaDataInput>();
            builder.RegisterType<MetaDataType>();
            builder.RegisterType<SubscriptionDocumentType>();
            

            //   builder.RegisterType<MyFieldRecordRegistration>().As<IFieldRecordRegistration>();
            //   builder.RegisterType<MyFieldRecordRegistration2>().As<IFieldRecordRegistration>();
        }
    }
}