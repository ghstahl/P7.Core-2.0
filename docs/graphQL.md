# GraphQL Support in P7
Built into P7 is GraphQL support using the following open source project;  
[graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet)  
Admittedly this was the first one I looked at, and at the time of usage it was pretty complete.  What drew me to it was that it had a nice way to apply security on the incoming requests. 

P7 provides a way to easily implement new graphQL endpoints.  Currently I am sweeping the project for implementations and registering them all with the DI, but I think I am going to require folks to register their implementations themselves.  Securing access to these queries is done via configuration, and that work is currently in progress.

P7 provides a stock graphQL implementation for fetching strings out of the ASP.NET RESX files.  There is also a custom REST api to fetch the same data, but where we can migrate away from REST to graphQL we should do it.  
[GraphQL resource implementation](../src/P7.Globalization)

## GraphQL Authorization 
Click [here](graphQL-Auth.md) to find out how.

## GraphQL Viewer
1. Browse to https://localhost:44311/GraphQLView

**Query**

### Query for browser culture
```graphql
query q($id: String!,$treatment: String! ){
  resource(input: { id: $id, treatment: $treatment })
}
```
**Query Variables**
```graphql
{
    "id": "P7.Main.Resources.Main,P7.Main",
  	"treatment":"kva"
}
or
{
    "id": "P7.Main.Resources.Main,P7.Main",
  	"treatment":"kvo"
}
```  
**Result**
```graphql
{
  "data": {
    "resource": [
      {
        "key": "Hello",
        "value": "Hello"
      }
    ]
  }
}
or
{
  "data": {
    "resource": {
      "hello": "Hello"
    }
  }
}
```  

### Query for any culture

```graphql
query q($id: String!,$treatment: String!,$culture: String!){
  resource(input: { id: $id, treatment: $treatment,culture: $culture  })
}
```
**Query Variables**
```graphql
{
    "id": "P7.Main.Resources.Main,P7.Main",
    "treatment":"kva",
    "culture":"fr-FR"
}
or
{
    "id": "P7.Main.Resources.Main,P7.Main",
    "treatment":"kvo",
    "culture":"fr-FR"
}
```  

**Result**
```graphql
{
  "data": {
    "resource": [
      {
        "key": "Hello",
        "value": "Bonjour"
      }
    ]
  }
}
or
{
  "data": {
    "resource": {
      "hello": "Bonjour"
    }
  }
}
```  

### The Blog Store
#### Insert via Mutation

```graphql
mutation q($input: blogMutationInput!) {
  blog(input: $input)
}
```
**Mutation Variables**
```graphql
{
    "input": {
	"metaData": {
		"category": "c0",
		"version": "1.0.0.0"
	},
	"document": {
		"categories": ["c10", "c20"],
		"tags": ["t10", "t20"],
		"data": "This is my blog",
		"timeStamp": "2027-03-15T20:01:11Z",
		"title": "My Title",
		"summary": "My Summary"
	},
	"id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62"
    }
}
```  
**Result**
```graphql
{
  "data": {
    "blog": true
  }
}
```

#### Query an individual Blog Entry
```graphql
query q($input: blogQueryInput!) {
  blog(input: $input) {
    tenantId
    id
    metaData {
      category
      version
    }
    document {
      title
      summary
      categories
      tags
      timeStamp
      data
    }
  }
}
```
**Query Variables**
```graphql
{
   "input": {
	"id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62"
   }
}
```  
**Result**
```graphql
{
  "data": {
    "blog": {
      "tenantId": "02a6f1a2-e183-486d-be92-658cd48d6d94",
      "id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
      "metaData": {
        "category": "c0",
        "version": "1.0.0.0"
      },
      "document": {
        "title": "My Title",
        "summary": "My Summary",
        "categories": [
          "c10",
          "c20"
        ],
        "tags": [
          "t10",
          "t20"
        ],
        "timeStamp": "2027-03-15T20:01:11Z",
        "data": "This is my blog"
      }
    }
  }
}
```
#### Javascript example
```
getBlog() {
    var body = JSON.stringify(
      {
          query: 'query q($input: blogQueryInput!) {blog(input: $input) {document {title summary categories tags timeStamp data}}}',
          variables: '{"input": {"id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62"}}',
          operationName: 'q'
      }
  );

  
    return new Promise((resolve, reject) => {
      fetch('https://pingo7api.azurewebsites.net/api/graphql', {
        credentials: 'include',
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: body
      }).then((res) => {
        res.json().then((res) => {
          console.log('success fetching', res);
          resolve(res.data);
        });
      }).catch((err) => {
        console.log('error fetching', err);
        reject(err);
      });
    });
  },
```
#### Paging Blogs
#### Query a page of Blog Entries
```graphql
query q($input: blogsPageQueryInput!) {
  blogsPageByNumber(input: $input) {
    tenantId
    id
    metaData {
      category
      version
    }
    document {
      title
      summary
      categories
      tags
      timeStamp
      data
    }
  }
}
```
**Query Variables**
```graphql
{
  "input": {"page": 1,"pageSize": 3}
}
```  
**Result**
```graphql
{
  "data": {
    "blogsPageByNumber": [
      {
        "tenantId": "02a6f1a2-e183-486d-be92-658cd48d6d94",
        "id": "6c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
        "metaData": {
          "category": "c0",
          "version": "1.0.0.0"
        },
        "document": {
          "title": "My Title 2020",
          "summary": "My Summary",
          "categories": [
            "c10",
            "c20"
          ],
          "tags": [
            "t10",
            "t20"
          ],
          "timeStamp": "2020-03-15T20:01:11Z",
          "data": "This is my blog"
        }
      },
      {
        "tenantId": "02a6f1a2-e183-486d-be92-658cd48d6d94",
        "id": "1c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
        "metaData": {
          "category": "c0",
          "version": "1.0.0.0"
        },
        "document": {
          "title": "My Title 2021",
          "summary": "My Summary",
          "categories": [
            "c10",
            "c20"
          ],
          "tags": [
            "t10",
            "t20"
          ],
          "timeStamp": "2021-03-15T20:01:11Z",
          "data": "This is my blog"
        }
      },
      {
        "tenantId": "02a6f1a2-e183-486d-be92-658cd48d6d94",
        "id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
        "metaData": {
          "category": "c0",
          "version": "1.0.0.0"
        },
        "document": {
          "title": "My Title",
          "summary": "My Summary",
          "categories": [
            "c10",
            "c20"
          ],
          "tags": [
            "t10",
            "t20"
          ],
          "timeStamp": "2027-03-15T20:01:11Z",
          "data": "This is my blog"
        }
      }
    ]
  }
}
```
## Subscription Query Tests
```
query q($input: subscriptionQueryInput!) {
  subscription(input: $input) {
    id
    metaData {
      category
      version
    } 
    value
  }
}
```
**Query Variables**
```graphql
{
	"input": {
		"metaData": {
			"category": "c0",
			"version": "1.0.0.0"
		},
		"id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62"
	}
}
```  
**Result**
```graphql
{
  "data": {
    "subscription": {
      "id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
      "metaData": {
        "category": "c0",
        "version": "1.0.0.0"
      },
      "value": {
        "subscriptionQueryHandle": {
          "id": "5c6b2b4c-f1c7-4f8d-a97b-1755c7d1fa62",
          "metaData": {
            "category": "c0",
            "version": "1.0.0.0"
          }
        },
        "ted": "Well Hellow"
      }
    }
  }
}
```  
## Identity Query Tests
```
query q($id: String!){
  identity(input: { id: $id }) {
    access_code
    oidc
  }
}
```
**Query Variables**
```graphql
{
    "id": "some-id"
}
```  
**Result**
```graphql
{
  "data": {
    "identity": {
      "access_code": "blah",
      "oidc": {
        "access_token": "ya29.GlwGBSeKM8_mGsDhVwHV4dvRjiHBZ-TqeVQMnVEJk6bM2M44Tw8ecaPRiOEy_UiSmK6UKbyDS5Fqt9uLN1BNW-1p4HiXcQvyC6bBfUABaOrEJfHaBziJfUfzpoDYsA",
        "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNlNzYwZGZmNDgxZWU5YmNhNDVjY2FiNjRlYWRhMzI4MDI5YmMwYWEifQ.eyJhenAiOiIxMDk2MzAxNjE2NTQ2LWVkYmw2MTI4ODF0N3JrcGxqcDNxYTNqdW1pbnNrdWxvLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiMTA5NjMwMTYxNjU0Ni1lZGJsNjEyODgxdDdya3BsanAzcWEzanVtaW5za3Vsby5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjEwNDc1ODkyNDQyODAzNjY2Mzk1MSIsImF0X2hhc2giOiJ2aUVhNjU3bXNkMFZ6OEUxMVdhV0F3Iiwibm9uY2UiOiI2MzY0NjQ0NTcwOTQwMjYyMzUuTWpRellUaGlPR1l0TkRGbE1TMDBOamhpTFRsbU1XTXROVGhpTkdZNE5EZGtNMkpqTXpobU5UUmhOVEF0WmpBeU5TMDBZekUwTFdGa05EUXRObU5rTWpCaVpEaGtNREEwIiwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tIiwiaWF0IjoxNTEwODQ4OTA5LCJleHAiOjE1MTA4NTI1MDksIm5hbWUiOiJIZXJiIFN0YWhsIiwicGljdHVyZSI6Imh0dHBzOi8vbGg0Lmdvb2dsZXVzZXJjb250ZW50LmNvbS8tdXZPc3RBRzhUcWsvQUFBQUFBQUFBQUkvQUFBQUFBQUFGVTAvaU9OSWpKbjNkZHMvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IkhlcmIiLCJmYW1pbHlfbmFtZSI6IlN0YWhsIiwibG9jYWxlIjoiZW4ifQ.lj2VXZDgvO-Rhg3tGg4XwPW6ybN2SHa-BnExLhiOnwxGKaE0PNjnFKsmSyFBqy3rpUlxzYcMZiAMC7CLMfK3Fi3MxWQjfvBQ8MDtegGjiDX46p7aGaxCJ8AHunm0vSiVBS4Sc1lbP3wnREvMc2_dBqdU0hCJbZKEuCBAVZrp5zXLQGnDMWxUueOsazAyUi9pduahK7wopCdd2MmcCqDMJcGgJh40hvkD31ZHhlsBFf68DIo2mnGG4JuhBviPci8zNrhizyWjqavub-LvPOM7cZrFRbI3oSNFkn8kRw5Dy3hSuNU0o_c3HQWmTiCO_bWcx2HoeVXpeP0mf01O49rDPg",
        "token_type": "Bearer",
        "expires_at": "2017-11-16T09:15:09-08:00"
      }
    }
  }
}
```  

