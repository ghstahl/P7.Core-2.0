using Autofac;
using P7.BlogStore.Core.GraphQL;
using P7.BlogStore.Core.Models;

namespace P7.BlogStore.Core
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlogQueryInput>();
            builder.RegisterType<BlogsQueryInput>();
            builder.RegisterType<BlogMutationInput>();
            builder.RegisterType<MetaDataType>();
            builder.RegisterType<Blog>();
            builder.RegisterType<BlogComment>();
            builder.RegisterType<BlogType>();
            builder.RegisterType<BlogPage>();
            builder.RegisterType<BlogPageType>();
            builder.RegisterType<MetaDataInput>();
            builder.RegisterType<BlogInput>();
            builder.RegisterType<BlogDocumentType>();
            builder.RegisterType<BlogsPageQueryInput>();
        }
    }
}