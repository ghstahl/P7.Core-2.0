using System.Collections.Generic;
using Autofac;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace ReferenceWebApp.CookieAuthApi
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var configuration = GlobalConfigurationRoot.Configuration;

            var authority = configuration["oauth2:norton:authority"];

            var additionalEndpointBaseAddresses = new List<string>();
            configuration.GetSection("oauth2:norton:additionalEndpointBaseAddresses").Bind(additionalEndpointBaseAddresses);


            var discoveryClient = new DiscoveryClient(authority);
            foreach (var additionalEndpointBaseAddress in additionalEndpointBaseAddresses)
            {
                discoveryClient.Policy.AdditionalEndpointBaseAddresses.Add(additionalEndpointBaseAddress);
            }
            var nortonDiscoveryCache = new DiscoveryCache(discoveryClient);

            builder.Register(c => nortonDiscoveryCache)
                .As<DiscoveryCache>()
                .SingleInstance();
        }
    }
}