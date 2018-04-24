# Decentralized Token Management Support
## Discovery
```
https://localhost:44312/.well-known/openid-configuration
produces:

{
	"issuer": "https://localhost:44312",
	"jwks_uri": "https://localhost:44312/.well-known/openid-configuration/jwks",
	"authorization_endpoint": "https://localhost:44312/connect/authorize",
	"token_endpoint": "https://localhost:44312/connect/token",
	"userinfo_endpoint": "https://localhost:44312/connect/userinfo",
	"end_session_endpoint": "https://localhost:44312/connect/endsession",
	"check_session_iframe": "https://localhost:44312/connect/checksession",
	"revocation_endpoint": "https://localhost:44312/connect/revocation",
	"introspection_endpoint": "https://localhost:44312/connect/introspect",
	"frontchannel_logout_supported": true,
	"frontchannel_logout_session_supported": true,
	"backchannel_logout_supported": true,
	"backchannel_logout_session_supported": true,
	"scopes_supported": ["arbitrary", "offline_access"],
	"claims_supported": [],
	"grant_types_supported": ["authorization_code", "client_credentials", "refresh_token", "implicit", "password", "public_refresh_token"],
	"response_types_supported": ["code", "token", "id_token", "id_token token", "code id_token", "code token", "code id_token token"],
	"response_modes_supported": ["form_post", "query", "fragment"],
	"token_endpoint_auth_methods_supported": ["client_secret_basic", "client_secret_post"],
	"subject_types_supported": ["public"],
	"id_token_signing_alg_values_supported": ["RS256"],
	"code_challenge_methods_supported": ["plain", "S256"]
}
```

P7 will support a decentralized token management system using the following open source project as the core engine;  
[OpenID Connect and OAuth 2.0 Framework for ASP.NET Core](https://github.com/IdentityServer/IdentityServer4)  

The service being provided is the creation of tokens and the management of those tokens in flight.  

The interesting thing is what is NOT being considered as part of the service, and those are as follows;  
1. Users  
2. Scopes  
3. Claims
4. id_token is NOT SUPPORTED

What I have found is that those artifacts tend to be decentralized, especially in large enterprises.  Inside enterprises, there are multiple business units, each with their own concept of what a user is and more importantly what a user has access to.  Basically one business inside an enterprise has nothing to do with the workings of another, and if by coincidence a single user does business with more than one there still should not be an association between the business units.

### What can P7 do for you?  
P7 will manage your OAuth 2.0 tokens, but will not be concerned of what is inside them.   
P7 will let you create arbitrary tokens, where what is inside them is passed into the system as arguments.  
P7 is not here to guard access to your resources, only to provide the inflight managment of tokens.  
Services that use P7 should have their own system of record for users, claims, and scopes that they manage.  It is those services that provide the real resources that clients want.

### Use Cases

Please visit [JWT.IO](https://jwt.io/) to interigate what is inside the access_tokens I am creating below.

#### The ability to create a Client_Credentials Flow type token
I need to be able to create a Client_Credentials token, where I can pass in an arbitrary user, with arbitrary scopes, and abitrary claims.  I need the service to then manage that token whilst in flight.
```
https://localhost:44312/connect/token POST

grant_type:client_credentials
scope:arbitrary
client_id:client
client_secret:secret
handler:arbitrary-claims-service
arbitrary-claims:{"naguid":"1234abcd","In":"Flames"}
arbitrary-scopes:A quick brown fox
namespace:p7-services
```
produces
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc4OGRhZjMxYmRjYWUwMzE1MWUwYzRkMWQ1MDNjOTU0IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ1OTAyODUsImV4cCI6MTUyNDU5Mzg4NSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIvcmVzb3VyY2VzIiwiYXJiaXRyYXJ5Il0sImNsaWVudF9pZCI6ImNsaWVudCIsIm5hZ3VpZCI6IjEyMzRhYmNkIiwiSW4iOiJGbGFtZXMiLCJhcmJpdHJhcnktbmFtZXNwYWNlIjoicDctc2VydmljZXMiLCJzY29wZSI6WyJhcmJpdHJhcnkiLCJBIiwicXVpY2siLCJicm93biIsImZveCJdfQ.T1MlY-Uhaa6yNnjg5nueRXFqHhb2FkkLg567YwgxOf88lO0euEhl1uE-0Zuy6cyH5YTkp3kKW6V-dvndzQ28Htj3XF-moRsajV65EVIVfLxl6UwYEnzWB331UF1q1l3bi9cPF2SXid8bOnKiVYR6Avi24P33pdJZY3miwf71seBdUf_italcr81IGuPDfndk_JH9hf1VMA4LuQ2Ieu7Y6BXSQoF0Npsx_hwnJIhcwmC57vc1IroFVRZMDRwYmJkRHdh1I5KvRCs_iii1vo3wtCzDQN1HsfzFAiGo0NOyAi2b3OIUH1lpw_ColwBATn7UHY24Fe8s5smOjkrOHJuckw",
    "expires_in": 3600,
    "token_type": "Bearer"
}
```
#### The ability to create a Resource_Owner Flow type token
I need to be able to create a Resource_Owner token, where I can pass in an arbitrary user, with arbitrary scopes, and abitrary claims.  I need the service to then manage that token whilst in flight.

``` 
Probably should make this an enhanced grant as well.  The username is carried through, but the password can be anything.
https://localhost:44312/connect/token POST

grant_type:password
scope:arbitrary offline_access
client_id:resource-owner-client
client_secret:secret
handler:arbitrary-claims-service
arbitrary-claims:{"some-guid":"1234abcd","In":"Flames"}
arbitrary-scopes:A quick brown fox openid
username:rat
password:poison
namespace:p7-services
```
produces
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc4OGRhZjMxYmRjYWUwMzE1MWUwYzRkMWQ1MDNjOTU0IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ1OTA5ODAsImV4cCI6MTUyNDU5NDU4MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIvcmVzb3VyY2VzIiwiYXJiaXRyYXJ5Il0sImNsaWVudF9pZCI6InJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6InJhdCIsImF1dGhfdGltZSI6MTUyNDU5MDk3OCwiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwiYXJiaXRyYXJ5LW5hbWVzcGFjZSI6InA3LXNlcnZpY2VzIiwic2NvcGUiOlsiYXJiaXRyYXJ5Iiwib2ZmbGluZV9hY2Nlc3MiLCJBIiwicXVpY2siLCJicm93biIsImZveCIsIm9wZW5pZCJdLCJhbXIiOlsicHdkIl19.pPQXOzDny39bXnFPzP-nk7OpTbdsHzNGRs8Sn7I5e6c-9B1JkZ87Anve70lmg1SVJxMerV_jURb0QRRCtR6y5ceQjm6VKL0qAga7IniGmpmZrztJ_koCPaQdvC0CWq7Rogl5-b7g_RnTZ_RhcLj7sGGKc8DctcdZOrqfbrOhIFAK5iyBJrNe-XluA59imvLxXEAY4kSI_h3oJTLBxv9UpEQMsoCnb-TmiYv8uGhN3_SJxQfis-4WO1-A7Z2dtjMb_C8gCZApyAoyLHupgH5Bo1PKDB0rirCImjFm-SAdQqY7pWYmzmLvZEGWN9rdPqqj4ik95PTnzzHEQviUkgU4sQ",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "7e6e4dd9ca70df9747b7ce9c3b609c21569a42bbf5dce7bad2fcc4be5d90d1b8"
}
```
#### The ability to refresh a Resource_Owner Flow type token

taking the refresh_token from above;
```
https://localhost:44312/connect/token POST

refresh_token:7e6e4dd9ca70df9747b7ce9c3b609c21569a42bbf5dce7bad2fcc4be5d90d1b8
client_id:resource-owner-client
```
produces the following;
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc4OGRhZjMxYmRjYWUwMzE1MWUwYzRkMWQ1MDNjOTU0IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ1OTA5OTIsImV4cCI6MTUyNDU5NDU5MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIvcmVzb3VyY2VzIiwiYXJiaXRyYXJ5Il0sImNsaWVudF9pZCI6InJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6InJhdCIsImF1dGhfdGltZSI6MTUyNDU5MDk3OCwiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwiYXJiaXRyYXJ5LW5hbWVzcGFjZSI6InA3LXNlcnZpY2VzIiwic2NvcGUiOlsiYXJiaXRyYXJ5Iiwib2ZmbGluZV9hY2Nlc3MiLCJBIiwicXVpY2siLCJicm93biIsImZveCIsIm9wZW5pZCJdLCJhbXIiOlsicHdkIl19.YfCB_yZGudxSXMCezUGgyTlPsCUvrDWw_vOAFcpuJC8kKs8oI902cHelKxNjU7aDJRm3W8RiXi1rsAtLA4BYAoswXFIylPFHD9Tv9G06AmfrU3UyC91ix8SqtPs3gmvPgYta9EH-AyKKQJOSRpgvGUA3Zaoyxkc0s37S-S8DFkcCSHx3pwJGzbhnj8Br9_gx3mMQBpdFb2yi8NYB2hP88LRO7GnPJpab6SaYNG2Y-YbAc9shfJuBBARtZnyd5rC3iR7QH1PZhdVOGCH94gGWDoJ77r7M1vndhwLSuhYCa4gx_jgG3T83nUoc6W5RmXF3GYRtBFkX7hvnJ75dmBdj7g",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "c471580d716d443c9e5c6535678ea1442e32bd5502fffa200e0d347f9b8a8f05"
}
```

## How is this accomplished.

There are 2 clients in play;

The resource_owner client requires that you must use a client_id, client_secret, username, and password to get the initial response.
The fact that it is configured to require a password, is what makes it not usable to use the refresh_token in the response in a public way.
```
    new Client
    {
        ClientId = "resource-owner-client",
        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        AllowOfflineAccess = true,
        RefreshTokenUsage = TokenUsage.OneTimeOnly,
        ClientSecrets =
        {
            new Secret("secret".Sha256())
        },
        AllowedScopes = { "arbitrary" }
    };

```
```
https://localhost:44311/connect/token POST
grant_type=client_credentials&scope=arbitrary&client_id=client&client_secret=secret&handler=arbitrary-claims-service&arbitrary-claims={"naguid":"1234abcd","In":"Flames"}&arbitrary-scopes=A quick brown fox
```

The next problem is how do we get to use the refresh_token that came back from that above request without passing a client_secret?
Enter a new client, but by convention its name and settings must be exact.
```
    new Client
    {
        ClientId = "public-resource-owner-client",
        AllowedGrantTypes = GrantTypes.List("public_refresh_token"),
        RequireClientSecret = false,
        AllowedScopes = { "arbitrary" }
    };

```
The ClientId must be "public-" + the clientId of the client whom you would like the refresh_token to be public.
RequireClientSecret must be set to false, and we now have an extension_grant that will help use refresh the token called "public_refresh_token".  
```
https://localhost:44311/connect/token POST
refresh_token=37c43e936af65423bbc62a28dcd9505a008203eeddc75d1043a33be0547ad075&client_id=resource-owner-client
```
There is nothing in this request that gives a user a hint as to the backend things in play.

### Under the Hood
1. [PublicRefreshTokenMiddleware](../src/P7.IdentityServer4.Common/Middleware/PublicRefreshTokenMiddleware.cs)  
This intercepts all Requests and is looking for /connect/token, with a client_id and refresh_token in the form.   It then sees if there is a public-{client_id} variant, and if there is we fixup the form data which routes the request to our public_refresh_token extension_grant implementation.  
2. [PublicRefreshTokenExtensionGrantValidator](../src/P7.IdentityServer4.Common/ExtensionGrantValidator/PublicRefreshTokenExtensionGrantValidator.cs)    
This extension understands the naming convention scheme and final result we are going for.  It bascially strips away the public- part of the client name and refreshes the token of the original client, but this time without requireing a client_secret.

3. The actual response back from IdentitySever4 is not exactly as we like it, so we read the resonse body and correct that to look nice.  This is handled in the [PublicRefreshTokenMiddleware](../src/P7.IdentityServer4.Common/Middleware/PublicRefreshTokenMiddleware.cs) middleware as well.  


### Important.
Don't forget to add the PublicRefeshTokenMiddleware, as it changes the incoming client_id to one that has public- prepended to it.

    ```
    app.UsePublicRefreshToken();
    app.UseIdentityServer();
    ```

## Security  

Having a token management system that lets you make a token with any claim and scope you want is a security hole.  It allows another client on the system to spoof another client's claims and scopes.  To stop this, we have introduced the ability for a client to stake a claim to any claim and scope name.  Its a first come first served situation.  If a client requests a token that has a claim or scope that someone has staked a claim to, that calling client gets nothing.  We don't leak the why they didn't get anything.  

```
Register the private provider at startup.  

builder.RegisterType<InMemoryPrivateClaimsScopesStore>()
    .AsSelf()
    .As<IPrivateClaimsScopesValidation>()
    .SingleInstance();
```
```
Populate it with test data.  
var privateStore = P7.Core.Global.ServiceProvider.GetServices<InMemoryPrivateClaimsScopesStore>().FirstOrDefault();

privateStore.AddPrivateScopes("Bjorn",new string[]{"flames"});
privateStore.AddPrivateClaims("Bjorn", new string[] { "bullet" });
```

Any client that is NOT "Bjorn", requesting a scope of "flames" or a claim or "bullet", will get nothing.
Only "Bjorn" gets to have a scope called "flames" and a claim called "bullet".
