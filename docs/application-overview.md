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
    


# Running Things  
1. ReferenceWebApp.ExternalIdentity  
   For me this app is running at https://localhost:44311/  
   This is a dual purpuse Token Management and Normal WebApp.  
   
2. ReferenceWebApp.Api.Only 
   For me this app is running at https://localhost:44313/Home/Contact  
   
# API Tests 
1. Getting an access Token; 
```
   POST https://localhost:44311/connect/token
   Content-Type: application/x-www-form-urlencoded
   grant_type=password&scope=arbitrary offline_access&client_id=resource-owner-client&client_secret=secret&handler=arbitrary-claims-service&arbitrary-claims={"naguid":"1234abcd","In":"Flames","bdullet":"Ride"}&username=rat&password=poison&arbitrary-scopes=A quick brown fox Fladmes
```
   Produces;  
```
{
"access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijk0NGIwOGMxZTcwZDdkNGQxYjJmZGNkMWQ3OGM0MDBhIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTA1ODg0NjQsImV4cCI6MTUxMDU5MjA2NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTEiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTEvcmVzb3VyY2VzIiwiYXJiaXRyYXJ5Il0sImNsaWVudF9pZCI6InJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6InJhdCIsImF1dGhfdGltZSI6MTUxMDU4ODQ2NCwiaWRwIjoibG9jYWwiLCJuYWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwiYmR1bGxldCI6IlJpZGUiLCJzY29wZSI6WyJhcmJpdHJhcnkiLCJvZmZsaW5lX2FjY2VzcyIsIkEiLCJxdWljayIsImJyb3duIiwiZm94IiwiRmxhZG1lcyJdLCJhbXIiOlsicHdkIl19.R1gq1kM5MlgY41P7gkC7ca8fs76-ITXmFQ4L5_a_NfsWhf25yoVLkh0y4Oj-iRXuiDPdPc81vo7fhpgcP5qD1cSlyVEQim0OgkNqcPhrJWHA3pI00jTAvRoR70R6C0Z4bsyy_aEq6HT3aOY1H279TiQIbYGl4qLzE9k7Etira_Wrbp2tSuvBYBi73eiIlL7JtrmyF2fo-4pAPxNX8vIOqEbOtOBUSnXWLj0RtjhfeqbX_1OJmeE3iVtXa3VVQOExW88cngZ46tb1VudL55UqEPVe_Rh3HnCu-l25qhHWcGRD9dDKqDc5n491aIzyGvKpqcOaalcQE9bQB1y20aH9eQ",
"expires_in": 3600,
"token_type": "Bearer",
"refresh_token": "0a6e0d3946a6159ae93ddcd7a049c6060e165b482d5a59593065932f724d9bfc"
}
```
   You can view the contents of the access_code at [jwt.io](https://jwt.io)  

2. Call a Demo API;  
   Copy the access_code from above into the Bearer slot.  
```
   GET https://localhost:44313/api/IdentityApi  
   Content-Type: application/json
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6Ijk0NGIwOGMxZTcwZDdkNGQxYjJmZGNkMWQ3OGM0MDBhIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTA1ODg0NjQsImV4cCI6MTUxMDU5MjA2NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTEiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTEvcmVzb3VyY2VzIiwiYXJiaXRyYXJ5Il0sImNsaWVudF9pZCI6InJlc291cmNlLW93bmVyLWNsaWVudCIsInN1YiI6InJhdCIsImF1dGhfdGltZSI6MTUxMDU4ODQ2NCwiaWRwIjoibG9jYWwiLCJuYWd1aWQiOiIxMjM0YWJjZCIsIkluIjoiRmxhbWVzIiwiYmR1bGxldCI6IlJpZGUiLCJzY29wZSI6WyJhcmJpdHJhcnkiLCJvZmZsaW5lX2FjY2VzcyIsIkEiLCJxdWljayIsImJyb3duIiwiZm94IiwiRmxhZG1lcyJdLCJhbXIiOlsicHdkIl19.R1gq1kM5MlgY41P7gkC7ca8fs76-ITXmFQ4L5_a_NfsWhf25yoVLkh0y4Oj-iRXuiDPdPc81vo7fhpgcP5qD1cSlyVEQim0OgkNqcPhrJWHA3pI00jTAvRoR70R6C0Z4bsyy_aEq6HT3aOY1H279TiQIbYGl4qLzE9k7Etira_Wrbp2tSuvBYBi73eiIlL7JtrmyF2fo-4pAPxNX8vIOqEbOtOBUSnXWLj0RtjhfeqbX_1OJmeE3iVtXa3VVQOExW88cngZ46tb1VudL55UqEPVe_Rh3HnCu-l25qhHWcGRD9dDKqDc5n491aIzyGvKpqcOaalcQE9bQB1y20aH9eQ
```  
   Produces;  
```
[{"type":"nbf","value":"1510588464"},{"type":"exp","value":"1510592064"},{"type":"iss","value":"https://localhost:44311"},{"type":"aud","value":"https://localhost:44311/resources"},{"type":"aud","value":"arbitrary"},{"type":"client_id","value":"resource-owner-client"},{"type":"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier","value":"rat"},{"type":"auth_time","value":"1510588464"},{"type":"http://schemas.microsoft.com/identity/claims/identityprovider","value":"local"},{"type":"naguid","value":"1234abcd"},{"type":"In","value":"Flames"},{"type":"bdullet","value":"Ride"},{"type":"scope","value":"arbitrary"},{"type":"scope","value":"offline_access"},{"type":"scope","value":"A"},{"type":"scope","value":"quick"},{"type":"scope","value":"brown"},{"type":"scope","value":"fox"},{"type":"scope","value":"Fladmes"},{"type":"http://schemas.microsoft.com/claims/authnmethodsreferences","value":"pwd"}]
```  

## Security  


   
