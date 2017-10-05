# P7.Core-2.0
An opinionated way to build Asp.Net Core applications.  
Using [.Net Core 2.0](https://www.microsoft.com/net/download/core)  


## Development

From a console

```cmd
:> git clone https://github.com/ghstahl/P7.Core-2.0.git
:> cd {P7.Core-2.0}/src
:> npm install
:> cd {P7.Core-2.0}\src\WebApplication1
:> gulp watch
```
This will monitor plugin projects that have static assests and copy them to the main webapp project.
This is only done during developement, as the goal is to have your plugin projects packaged up as nugets.
Your nugets will be versioned and contain your assemblies and static content.

## Build
This is a Visual Studio 2017+ Version 15.3.4 project  
Don't forget to install the [.Net Core 2.0 SDK](https://www.microsoft.com/net/download/core)  


### [GraphQL Support](docs/graphQL.md)  
### [Decentralized Token Management Support](docs/decentralized-token-management-support.md)
### [Opt-Out IFilterProvider](docs/opt-out-filter-provider.md)
### [Health Checks](docs/health-checks.md)
### [USING A CUSTOM HOSTNAME WITH IIS EXPRESS](docs/using-a-custom-hostname-with-iis-express-with-visual-studio-2017-vs2017.md)


## Credits
Microsoft OpenSource Asp.Net everything  
A ton of people from stackoverflow.com  
Those that contribute their own time to build great open source solutions.  

[aspnet](https://github.com/aspnet)  
[FakeItEasy](https://github.com/FakeItEasy/FakeItEasy)   
[ZeroFormatter](https://github.com/neuecc/ZeroFormatter)   
[IdentityServer4](https://github.com/IdentityServer/IdentityServer4)   
[graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet)   
[GraphQL-Parser](https://github.com/graphql-dotnet/parser)   
[graphiql](https://github.com/graphql/graphiql)   
[Autofac](https://github.com/autofac/Autofac)   
[Serilog](https://serilog.net/)   
[NewtonSoft Json.Net](https://www.newtonsoft.com/json)   
[Hangfire](https://www.hangfire.io/)   
[AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)   

