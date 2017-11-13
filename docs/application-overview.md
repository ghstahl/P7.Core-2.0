# Application Overview  

There are 3 applications in play when we talk about a production deployment;

1. **Token Managment Micrososervice ala IdentityServer 4**  
    **Project:** ReferenceWebApp.ExternalIdentity  
    
    This project is also a normal WebApp, but I would avoid double duty in production.
    
2. **API Only Web App**  
     **Project:** ReferenceWebApp.Api.Only   
     
    These APIs are gated by bearer tokens, which are managed by our Token Managment Microservice.  
    The APIs that you can put here are;  
    * Normal REST APIS
    * GraphQL 
    
3. **A normal WebApp which includes;**  
    **Project:** ReferenceWebApp.ExternalIdentity.NoIdentityServer4   
    
    * Serves normal HTML
    * Cookie based Auth REST APIs (REST and GraphQL)
    
