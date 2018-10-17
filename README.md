# graphql-net-client

Very simple GraphQL client for .NET/C#.
Requires JSON.NET!

## Making a request

```csharp
var client = new GraphQLClient("https://mygraphql.endpoint");
var query = @"query($Org:String!, $Items:Int!, $Cursor:String) {
    organization(first:$Items, after:$Cursor) {
        totalCount
        pageInfo {
            endCursor
            hasNextPage
        }
        nodes {
            name
            login
            avatarUrl
        }
    }
}";

var result = client.Query(query, new { Org = "some-org", Items = 10 });
```

## Making a request with the query builder

The query builder allows you to use a fluent interface to create a GraphQL query.

```csharp
using GraphQL.Client.QueryBuilder;
//....
var query = GraphQLQueryBuilder.New("query", arguments =>
{
    arguments.Var("Org").String().Required();
    arguments.Var("Items").Int().Required();
    arguments.Var("Cursor").String();
}).Query("organization", arguments =>
{
    arguments.Param("first", "$Items");
    arguments.Param("after", "$Cursor");
}, select =>
{
    select.Field("totalCount");
    select.Field("pageInfo").Select(pageInfo =>
    {
        pageInfo.Field("endCursor");
        pageInfo.Field("hasNextPage");
    });
    select.Field("nodes").Select(nodes =>
    {
        nodes.Field("name");
        nodes.Field("login");
        nodes.Field("avatarUrl");
    });
}).Build();

var result = client.Query(query, new { Org = "some-org", Items = 10 });
```

## Configure HTTP Headers

```csharp
var client = new GraphQLClient("https://mygraphql.endpoint", headers => {
    headers[HttpRequestHeader.ContentType] = "application/json";
    headers["X-Some-Header"] = "Some value";
});
```
## Use Json Linq to access result objects

```csharp
var result = client.Query(query, new { Org = "some-org", Items = 10 });

var nodes = result["organization"]["nodes"].ToObject<IEnumerable<MyObject>>();
```

## Get root object

```csharp
var result = client.Query(query, new { Org = "some-org", Items = 10 });

var root = result.DataAs<ResultObject>();
```

## Get raw data

```csharp
var result = client.Query(query, new { Org = "some-org", Items = 10 });

var rawString = result.Raw;
```

## Get response headers

```csharp
var result = client.Query(query, new { Org = "some-org", Items = 10 });

var headersDictionary = result.Headers;
```

## Get returned query errors

If the query contains errors, the Data object will be null and the Errors object will contain a list of errors reported by the GraphQl API.

```csharp
var result = client.Query(badQuery, new { Org = "some-org", Items = 10 });

var errors = result.Errors;
```