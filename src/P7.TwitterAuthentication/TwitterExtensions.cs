// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace P7.TwitterAuthentication
{
    public static class TwitterExtensions
    {
        public static AuthenticationBuilder AddP7Twitter(this AuthenticationBuilder builder)
            => builder.AddP7Twitter(TwitterDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddP7Twitter(this AuthenticationBuilder builder, Action<TwitterOptions> configureOptions)
            => builder.AddP7Twitter(TwitterDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddP7Twitter(this AuthenticationBuilder builder, string authenticationScheme, Action<TwitterOptions> configureOptions)
            => builder.AddP7Twitter(authenticationScheme, TwitterDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddP7Twitter(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TwitterOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TwitterOptions>, TwitterPostConfigureOptions>());
            return builder.AddRemoteScheme<TwitterOptions, TwitterHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
