using System.Collections.Generic;
using System.Reflection;
using Autofac;
using P7.IdentityServer4.BiggyStore;
using P7.IdentityServer4.Common;

namespace Test.P7.IdentityServer4.BiggyStore
{
    public class MyAutofacFactory
    {
        public IIdentityServer4BiggyConfiguration BiggyConfiguration { get; set; }

        private IContainer _autofacContainer;

        public IContainer AutofacContainer
        {
            get
            {
                if (_autofacContainer == null)
                {
                    var builder = new ContainerBuilder();
                    List<Assembly> assemblies = new List<Assembly>
                    {
                        Assembly.Load(new AssemblyName("P7.IdentityServer4.BiggyStore")),
                        Assembly.Load(new AssemblyName("P7.IdentityServer4.Common"))
                    };
                    builder.RegisterAssemblyModules(assemblies.ToArray());

                    builder.RegisterInstance(BiggyConfiguration).As<IIdentityServer4BiggyConfiguration>();
                    builder.RegisterType<ClientStore>().As<IFullClientStore>();

                    var container = builder.Build();

                    _autofacContainer = container;
                }

                return _autofacContainer;
            }
        }

        public T Resolve<T>()
        {
            ResolutionExtensions.Resolve<T>(AutofacContainer);
            return ResolutionExtensions.Resolve<T>(AutofacContainer);
        }
        public IEnumerable<T> ResolveMany<T>()
        {
            return Resolve<IEnumerable<T>>();
        }
    }
}