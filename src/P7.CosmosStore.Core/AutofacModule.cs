using Autofac;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using P7.CosmosStore.Core.GraphQL;
using P7.CosmosStore.Core.Models;

namespace P7.CosmosStore.Core
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<User>();
            builder.RegisterType<UserInput>();
            builder.RegisterType<SeatInput>();
            builder.RegisterType<EntitlementInput>();
            builder.RegisterType<UserMutationInput>();
            builder.RegisterType<UserQueryInput>();
        }
    }
}