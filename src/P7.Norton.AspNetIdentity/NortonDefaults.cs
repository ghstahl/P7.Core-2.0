namespace P7.Norton.AspNetIdentity
{
    public static class NortonDefaults
    {
        public static class Development
        {
            public static readonly string AuthorizationEndpoint = "https://login-int.norton.com/sso/idp/OIDC";

            public static readonly string TokenEndpoint = "https://login-int.norton.com/sso/oidc1/token";

            public static readonly string UserInformationEndpoint = "https://login-int.norton.com/sso/oidc1/userinfo";

            public static readonly string Authority = "https://login-int.norton.com/sso/oidc1/token";
        }

        public static class Production
        {
            public static readonly string AuthorizationEndpoint = "https://login.norton.com/sso/idp/OIDC";

            public static readonly string TokenEndpoint = "https://login.norton.com/sso/oidc1/token";

            public static readonly string UserInformationEndpoint = "https://login.norton.com/sso/oidc1/userinfo";

            public static readonly string Authority = "https://login.norton.com/sso/idp/OIDC";
        }

        public const string AuthenticationScheme = "Norton";

        public static readonly string DisplayName = "Norton";

    }
}