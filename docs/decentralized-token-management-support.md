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
arbitrary-claims:{"some-guid":"1234abcd","In":"Flames"}
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
I need to be able to create a Resource_Owner token, where I can pass in an arbitrary subject and abitrary claims.  I need the service to then manage that token whilst in flight.

``` 
Probably should make this an enhanced grant as well.  The username is carried through, but the password can be anything.
https://localhost:44312/connect/token POST

grant_type:arbitrary_owner_resource
scope:offline_access nitro
client_id:resource-owner-client
client_secret:secret
arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
subject:camera1
```
produces
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM1MjlmNDk0NzM5MGE4ZmE2ZThkMTJiOTEwMmI3Y2NjIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ4NTQ1NTAsImV4cCI6MTUyNDg1ODE1MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIvcmVzb3VyY2VzIiwibml0cm8iXSwiY2xpZW50X2lkIjoicmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiY2FtZXJhMSIsImF1dGhfdGltZSI6MTUyNDg1NDU1MCwiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X293bmVyX3Jlc291cmNlIl19.AwPNdFjZyzWfD1CFMhY2om-OuhtrS58PAo9Bn7AHInXp7YczSb6am7sX0ccMYLVuXw4670CUQiqF7EHHKaXVX-JFrhbFDdDj2VaQGFUOHwZ0nE-6A8WX5KQYuvRGmQ4YQnZQYsM0GLfjs9KBnXojloYrO5mxUgXOFEUzmaSJM18uW7X3ekaa28r6T_gHSXSS-t2v2PapjPAgMngc3y7moNgYaBlXFprvzAsouPMmAwzXVK0bUnb8qfqF9WlmYPyf9CCRFh9aKvUjGPcKHZerWwuT4IPnSoo_B4vbdKKZSh955zKt5YzqKMB7Aufjq26ZVua1Q8HctQNUqvziGWs1TQ",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "8e41003dc0d87ea8a2cb31bbb99fd34362227df58ea71aa532b52c778801429d"
}
```
#### The ability to refresh a Resource_Owner Flow type token

taking the refresh_token from above;
```
https://localhost:44312/connect/token POST

refresh_token:8e41003dc0d87ea8a2cb31bbb99fd34362227df58ea71aa532b52c778801429d
client_id:resource-owner-client
```
produces the following;
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM1MjlmNDk0NzM5MGE4ZmE2ZThkMTJiOTEwMmI3Y2NjIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MjQ4NTQ1NzMsImV4cCI6MTUyNDg1ODE3MywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTIvcmVzb3VyY2VzIiwibml0cm8iXSwiY2xpZW50X2lkIjoicmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoiY2FtZXJhMSIsImF1dGhfdGltZSI6MTUyNDg1NDU1MCwiaWRwIjoibG9jYWwiLCJzb21lLWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwic2NvcGUiOlsibml0cm8iLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsiYXJiaXRyYXJ5X293bmVyX3Jlc291cmNlIl19.QiNA8JKKxxdk9iJSCIVxojc-E5f1PGE0RwyQ_sDkkYmVtuvmEdlnJ5W7MlX538Ee7V4sAueAPG2w1_C1cFwFAxC7jtEh2vw9j_1uuBw1wwyugiZ1PifoOXygftXDarOXFuUFPg3KcRlODmxDqMUgZaChMToalifVlMXNb9evpt_jsTf_RasWllYlTSr0eu9x1djeAOaVWNzN9BR81bXm7P_-YQnaTdxJEUFGGuxh58xGkfhMqcjpumVkLESBW69Cd1yiAy_0vis_lwIq42F0Tu2geJ5WNcTWavdBT5suHP1HIgyES3Mh0jmyons99IfTUu3LNhdEVyYUQL6CgOomCQ",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "ff5d53c498d82094d0ebd49f0d3ad52793473f7bb938de8b45b96c12f4c45531"
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
grant_type=client_credentials&scope=arbitrary&client_id=client&client_secret=secret&handler=arbitrary-claims-service&arbitrary-claims={"some-guid":"1234abcd","In":"Flames"}&arbitrary-scopes=A quick brown fox
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

Having a token management system that lets you make a token with any claim and scope you want is a security hole.  It allows another client on the system to spoof another client's claims and scopes.  To stop this, we have introduced a namespace requirement where a client must be configured with a namespace.  Many clients can share the same namespace.  

The burden of checking will fall on the consumer of the token, as this namespace will show up as a claim;
```
arbitrary-namespace:{namespace}
```

```
Register the private provider at startup.  

 builder.RegisterType<InMemoryClientNamespaceValidationStore>()
                .AsSelf()
                .As<IClientNamespaceValidation>()
                .SingleInstance();
```

```
Populate it with test data.  

 var clientNamespaceValidationStore = 
                    P7.Core.Global.ServiceProvider.GetServices<InMemoryClientNamespaceValidationStore>()
                    .FirstOrDefault();
clientNamespaceValidationStore.AddClientNamespaces("resource-owner-client",new []{ "p7-services", "test" });
clientNamespaceValidationStore.AddClientNamespaces("resource-owner-client_2", new[] { "p7-services", "test" });
clientNamespaceValidationStore.AddClientNamespaces("client", new[] { "p7-services", "test" });
```


