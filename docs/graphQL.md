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
