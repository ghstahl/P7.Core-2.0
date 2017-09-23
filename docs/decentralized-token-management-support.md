# Decentralized Token Management Support

P7 will support a decentralized token management system using the following open source project as the core engine;  
[OpenID Connect and OAuth 2.0 Framework for ASP.NET Core](https://github.com/IdentityServer/IdentityServer4)  

The service being provided is the creation of tokens and the management of those tokens in flight.  

The interesting thing is what is NOT being considered as part of the service, and those are as follows;  
1. Users  
2. Scopes  
3. Claims  

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
http://localhost:7791/connect/token POST
grant_type=client_credentials&scope=arbitrary&client_id=client&client_secret=secret&handler=arbitrary-claims-service&arbitrary-claims={"naguid":"1234abcd","In":"Flames"}&arbitrary-scopes=A quick brown fox
```
produces
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjMzNzRmYjYxYWI2NWM3OTczMzViMWEyNzFiNjExNjE2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE0ODg1NTkzMTAsImV4cCI6MTQ4ODU2MjkxMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo3NzkxIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6Nzc5MS9yZXNvdXJjZXMiLCJhcmJpdHJhcnkiXSwiY2xpZW50X2lkIjoiY2xpZW50IiwibmFndWlkIjoiMTIzNGFiY2QiLCJJbiI6IkZsYW1lcyIsInNjb3BlIjpbImFyYml0cmFyeSIsIkEiLCJxdWljayIsImJyb3duIiwiZm94Il19.C0C8qD1vO9hzbmLKqvjhQ5p4b-uhAC5iEyTeBefh6MK9ba7tfq9s4Xa2sn-_3VhhEzwP4KVRxmyyUDI1rBj8qXPQ4AILuyVMyPnuLEH2k38eOk1ATuoLvTpQe4i2MiAlifymvxW2nbJhjH35928U5khL_7Pp7sG4mGyRD4ldFe544z7DLChhaCfWo6eVEjZfP02DnOsrWTx5o40E_EF9T8U1SOixdIkkSsCofnroNGjBpYh4CS4Ja_8c8UZKznDQ5KSQuaskgrqLn5840dzboo0Cyv-AKptR-KWsy_5gncFLGjIrdLsWCRhf3PvzxLow_tt4RdLaJT6x1iOP1FmzZg",
    "expires_in": 3600,
    "token_type": "Bearer"
}
```
#### The ability to create a Resource_Owner Flow type token
I need to be able to create a Resource_Owner token, where I can pass in an arbitrary user, with arbitrary scopes, and abitrary claims.  I need the service to then manage that token whilst in flight.

``` 
Probably should make this an enhanced grant as well.  The username is carried through, but the password can be anything.
http://localhost:7791/connect/token POST
grant_type=password&scope=arbitrary offline_access&client_id=resource-owner-client&client_secret=secret&handler=arbitrary-claims-service&arbitrary-claims={"naguid":"1234abcd","In":"Flames"}&username=rat&password=poison&arbitrary-scopes=A quick brown fox
```
produces
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImUzZWJiZTk5MzViNjI1NzkzNTE1MjZmMTYwYmQ2YmE2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE0ODkxNzE3NjcsImV4cCI6MTQ4OTE3NTM2NywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo3NzkxIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6Nzc5MS9yZXNvdXJjZXMiLCJhcmJpdHJhcnkiXSwiY2xpZW50X2lkIjoicmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoicmF0IiwiYXV0aF90aW1lIjoxNDg5MTcxNzY3LCJpZHAiOiJsb2NhbCIsIm5hZ3VpZCI6IjEyMzRhYmNkIiwiSW4iOiJGbGFtZXMiLCJzY29wZSI6WyJhcmJpdHJhcnkiLCJvZmZsaW5lX2FjY2VzcyIsIkEiLCJxdWljayIsImJyb3duIiwiZm94Il0sImFtciI6WyJwd2QiXX0.qqFwGOb36pxgcoxuxQ_91mU7a4nu_95krUu21AkdXBoeKFSyKVsvEV_vf3CHtpBV_0pxSocHZD-iipjujfI5BYegmtE-J3fdNUdoN5F9f3h9enUYWoTC1eo0gj1DDN1IAiT4oZcL05Mze49nSZ46S7bjN3xJpHo_-bOqJ94hggsgaTcjJw-r0ocGpwcd2u7pH8TiCrbZTLDqb6EOivsjUGJUgUwymaaoLxG6BxQTZ0JQs81uP_Psxesnwdolp6kj4PUc4OCx3FN8XOqAYQi2o_BHVsUvy_4Jq3teVxUrbWp2s-3vSQ0_R_EFm_s2dNdEyD7G12dJf2bgCWeVAaT5dA",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "37c43e936af65423bbc62a28dcd9505a008203eeddc75d1043a33be0547ad075"
}
```
#### The ability to refresh a Resource_Owner Flow type token

taking the refresh_token from above;
```
http://localhost:7791/connect/token POST
refresh_token=37c43e936af65423bbc62a28dcd9505a008203eeddc75d1043a33be0547ad075&client_id=resource-owner-client
```
produces the following;
```
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImUzZWJiZTk5MzViNjI1NzkzNTE1MjZmMTYwYmQ2YmE2IiwidHlwIjoiSldUIn0.eyJuYmYiOjE0ODkxNzE4NTUsImV4cCI6MTQ4OTE3NTQ1NSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo3NzkxIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6Nzc5MS9yZXNvdXJjZXMiLCJhcmJpdHJhcnkiXSwiY2xpZW50X2lkIjoicmVzb3VyY2Utb3duZXItY2xpZW50Iiwic3ViIjoicmF0IiwiYXV0aF90aW1lIjoxNDg5MTcxNzY3LCJpZHAiOiJsb2NhbCIsIm5hZ3VpZCI6IjEyMzRhYmNkIiwiSW4iOiJGbGFtZXMiLCJzY29wZSI6WyJhcmJpdHJhcnkiLCJvZmZsaW5lX2FjY2VzcyIsIkEiLCJxdWljayIsImJyb3duIiwiZm94Il0sImFtciI6WyJwd2QiXX0.UiL89PR35QiB-eX2KMDjtMLV80yL1IWR0XZMf9CkAsdIZEmptvy5w4uwqmpQBldYwT4q_Vtp5wRYWfer1_zhf3YCY5lQ21S-JLidz3Dkrd5pMrEaoiO3Ur0MyQMkFnVtj7uwuvKTxFJen3rAgbHC5b5VXGRspT0Kr0g0IgNh7EQWLdA_p6MAa5r4S9yXlkNHwz1rmTPBOB1a0zOZumMZYNZ5JmTI26dwGtUC0n5IlJB7NqD4O_4LZSlCOFJZwm2AqAJoCjylqKkOLvlv8YaQyL6-tRdH6q9x0VnFgn0m9pZqdMR9_CqCnRe7qbQrBuroBSs2i7cdiRmvcgIpAzvIMA",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "dfe6d6451f2879aa2bed087ae3b04f0a70134be4fc70a51970eb1e0de626b7a7"
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
http://localhost:7791/connect/token POST
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
http://localhost:7791/connect/token POST
refresh_token=37c43e936af65423bbc62a28dcd9505a008203eeddc75d1043a33be0547ad075&client_id=resource-owner-client
```
There is nothing in this request that gives a user a hint as to the backend things in play.

### Under the Hood
1. [PublicRefreshTokenMiddleware](../src/P7.IdentityServer4.Common/Middleware/PublicRefreshTokenMiddleware.cs)  
This intercepts all Requests and is looking for /connect/token, with a client_id and refresh_token in the form.   It then sees if there is a public-{client_id} variant, and if there is we fixup the form data which routes the request to our public_refresh_token extension_grant implementation.  
2. [PublicRefreshTokenExtensionGrantValidator](../src/P7.IdentityServer4.Common/ExtensionGrantValidator/PublicRefreshTokenExtensionGrantValidator.cs)    
This extension understands the naming convention scheme and final result we are going for.  It bascially strips away the public- part of the client name and refreshes the token of the original client, but this time without requireing a client_secret.

3. The actual response back from IdentitySever4 is not exactly as we like it, so we read the resonse body and correct that to look nice.  This is handled in the [PublicRefreshTokenMiddleware](../src/P7.IdentityServer4.Common/Middleware/PublicRefreshTokenMiddleware.cs) middleware as well.
