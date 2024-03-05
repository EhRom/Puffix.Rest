# Puffix REST

Library to handle REST API in your project.

The library handles the management of the HTTP client, authentication and result serialization.

Three samples are provided in the test projet (`Tests.Puffix.Rest`):

- Open Weather Map API
- Azure Maps API
- OVH API

## Token

The first element to implement is the token. Two types of token are available: the header token (`IHeaderToken` interface), when the token is passed in the request headers, or the query token (`IQueryParameterToken` interface), when the token is passed as a query parameter. These interfaces are inherited to define a contract and an implementatipn as shown in the following paragraphs.

### Header token

Create the token interface:

```csharp
public interface ISampleApiToken : IHeaderToken { }
```

Token implementation:

```csharp
public class SampleApiToken(IConfiguration configuration) : IOvhApiToken
{
    private readonly string token = configuration["tokenParamaterName"] ?? string.Empty;

    public IDictionary<string, string> GetHeaders()
    {
        return new Dictionary<string, string>()
        {
            { "headerName", token },
        };
    }
}
```

### Query parameter token

Create the token interface:

```csharp
public interface ISampleApiToken : IQueryParameterToken { }
```

Token implementation:

```csharp
public class SampleApiToken(IConfiguration configuration) : ISampleApiToken
{
    private readonly string token = configuration["tokenParamaterName"] ?? string.Empty;

    public string GetQueryParameter()
    {
        return $"queryParameterName={token}";
    }
}
```

### Query path token

Create the token interface:

```csharp
public interface ISampleApiToken : IQueryPathToken { }
```

Token implementation:

```csharp
public class SampleApiToken(IConfiguration configuration) : ISampleApiToken
{
    private readonly string token = configuration["tokenParamaterName"] ?? string.Empty;

    public string GetQueryPath()
    {
        return token;
    }
}
```

## Query description

The query description embeds and manages all the information for the query, like the base URI, query path, query parameters, query headers, or body. It controls the construction of the full URI with the path, the parameters, and the token, if needed. It also controls the body serialization or the headers sent to the targeted service. A contract is defined and inherits from the base contract, the `IQueryInformation<TokenT>` interface. Then a conctrete class is created and inherits from the defined contract, and the `QueryInformation<TokenT>` base class.

Create the query infotmation interface:

```csharp
public interface ISampleApiQueryInformation : IQueryInformation<ISampleApiToken> { }
```

Query information implementation:

```csharp
public class SampleApiQueryInformation(HttpMethod httpMethod, ISampleApiToken? token, IDictionary<string, string> headers, string baseUri, string queryPath, string queryParameters, string queryContent) :
    QueryInformation<ISampleApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    ISampleApiQueryInformation
{
    public static ISampleApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new SampleApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }

    public static ISampleApiQueryInformation  CreateNewAuthenticatedQuery(ISampleApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new SampleApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }
}
```

The query information implementation can override the `GetUriWithParameters`, `GetQueryContent` or `GetAuthenticationHeader` methods to have more control over the generation of these elements.

The `CreateNewAuthentication` method is used to build unauthenticated queries, for example, as a first call to get a token to a service.

## HTTP repository

The HTTP repository embeds the logic to implement the HTTP client to send the query and handle the response. To call the base HTTP repository, a contract is defined and inherits from the base contract, the `IRestHttpRepository<QueryInformationContainerT, TokenT>` interface. Then a conctrete class is created and inherits from the defined contract, and the `RestHttpRepository<QueryInformationContainerT, TokenT>` base class.

Create the query infotmation interface:

```csharp
public interface ISampleApiHttpRepository : IRestHttpRepository<ISampleApiQueryInformation, ISampleApiToken> { }
```

HTTP repository implementation:

```csharp
public class SampleApiHttpRepository(IHttpClientFactory httpClientFactory) :
    RestHttpRepository<ISampleApiQueryInformation, ISampleApiToken>(httpClientFactory),
    ISampleApiHttpRepository
{
    public override ISampleApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return SampleApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }

    public override ISampleApiQueryInformation BuildAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return SampleApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }
}
```

The HTTP repository is also used as a gateway to initialise the information objects in the request. Thus, in basic calls, only references to the repository and the basic token are required.

## Base call

```csharp
ISampleApiToken token = container.Resolve<ISampleApiToken>();
ISampleApiHttpRepository httpRepository = container.Resolve<ISampleApiHttpRepository>();

ISampleApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, apiBaseUri, queryPath, queryParameters, queryContent);

string response = await httpRepository.HttpAsync(queryInformation);
```

Complete samples are available in the `Tests.Puffix.Rest` project.
