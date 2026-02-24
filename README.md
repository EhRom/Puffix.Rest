f# Puffix REST

Library to handle REST API in your project.

[![Build and pusblish](https://github.com/EhRom/Puffix.Rest/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/EhRom/Puffix.Rest/actions/workflows/dotnet-core.yml)

![NuGet Version](https://img.shields.io/nuget/v/Puffix.Rest)

The library handles the management of the HTTP client, authentication and result serialization.

Four samples are provided in the test project (`Tests.Puffix.Rest`):

- **Open Weather Map** API
- **Azure Maps** API
- **OVH** API
- **ipify** API

> [!IMPORTANT]  
> Starting with version 10.1, a breaking change has been introduced to allow new types of queries, such as forms, binary content, etc., in addition to the classic string content. The classic *elements* `IQueryInformation`, `QueryInformation`, `IRestHttpRepository`, and `RestHttpRepository` are prefixed with `Basic`. The *old* names are reused to implement the improved behavior to handle more query content types.

## Token

The first element to implement is the **token**. Four types of token are available:

- the header token (`IHeaderToken` interface), when the token is passed in the request headers, 
- the query parameter token (`IQueryParameterToken` interface), when the token is passed as a query parameter.
- the query path token (`IQueryPathToken` interface), when the token is passed in the query path.
- the *empty* token (`IEmptyToken` interface), when no token is required for the API.

These interfaces inherit from the `IToken` interface, which defines the contract to be implemented as shown in the following paragraphs.

### Header token

Contract:

```csharp
public interface ISampleApiToken : IHeaderToken { }
```

Implementation:

```csharp
public class SampleApiToken(IConfiguration configuration) : ISampleApiToken
{
    private readonly string token = configuration["tokenParamaterName"] ?? string.Empty;

    public IDictionary<string, IEnumerable<string>> GetHeaders()
    {
        return new Dictionary<string, IEnumerable<string>>()
        {
            { "headerName", [token] },
        };
    }
}
```

### Query parameter token

Contract / interface:

```csharp
public interface ISampleApiToken : IQueryParameterToken { }
```

Implementation:

```csharp
public class SampleApiToken(IConfiguration configuration) : ISampleApiToken
{
    private readonly string token = configuration["tokenParamaterName"] ?? string.Empty;

    public string GetQueryParameterName()
    {
        return "queryParameterName";
    }

    public string GetQueryParameterValue()
    {
        return token;
    }
}
```

### Query path token

Contract / interface:

```csharp
public interface ISampleApiToken : IQueryPathToken { }
```

Implementation:

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

The query description embeds and manages all the information for the query information, like the HTTP method, headers, base URI, query path, query parameters, and body. It controls the construction of the full URI including the path, parameters, and token, if needed. It also adds the headers or controls the serialisation of the body sent to the targeted service. A contract is defined that inherits from the `IQueryInformation<TokenT>` base contract / interface. Then a concrete class is created that inherits from this contract, and the `QueryInformation<TokenT>` base class.

Contract / interface:

- Classic:

    ```csharp
    public interface ISampleApiQueryInformation : IBasicQueryInformation<ISampleApiToken> { }
    ```

- New: under implementation

Implementation:

- Classic:

    ```csharp
    public class SampleApiQueryInformation(HttpMethod httpMethod, ISampleApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
        BasicQueryInformation<ISampleApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
        ISampleApiQueryInformation
    {
        public static ISampleApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            return new SampleApiQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
        }

        public static ISampleApiQueryInformation  CreateNewAuthenticatedQuery(ISampleApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            return new SampleApiQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
        }
    }
    ```

- New: under implementation

The query information implementation can override the `GetUriWithParameters`, `GetQueryContent` or `GetAuthenticationHeader` methods to have more control over the generation of these elements. For example, this can be useful when generating a token that must include information about the generation date/time.

The `CreateNewUnauthenticatedQuery` method is used to build unauthenticated queries, for example, as a first call to get a token to a service.

## HTTP repository

The HTTP repository contains the logic to implement the HTTP client, to send the query, and to handle the response. To call the base HTTP repository, a contract is defined and it inherits from the base contract, the `IRestHttpRepository<QueryInformationContainerT, TokenT>` interface. Then a concrete class is created and it inherits from the defined contract, and the `RestHttpRepository<QueryInformationContainerT, TokenT>` base class.

Contract / interface:

- Classic:

    ```csharp
    public interface ISampleApiHttpRepository : IBasicRestHttpRepository<ISampleApiQueryInformation, ISampleApiToken> { }
    ```

- New: under implementation

Implementation:

- Classic:

    ```csharp
    public class SampleApiHttpRepository(IHttpClientFactory httpClientFactory) :
        BasicRestHttpRepository<ISampleApiQueryInformation, ISampleApiToken>(httpClientFactory),
        ISampleApiHttpRepository
    {
        public override ISampleApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
            return SampleApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
        }

        public override ISampleApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            return SampleApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
        }

        public override ISampleApiQueryInformation BuildAuthenticatedQuery(ISampleApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
            return SampleApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
        }

        public override ISampleApiQueryInformation BuildAuthenticatedQuery(ISampleApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
        {
            return SampleApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
        }
    }
    ```

- New: under implementation

The HTTP repository is also used as a gateway to initialise the information objects in the request. Thus, for basic calls, only references to the repository and the token are required.

> TODO Basic

## Base call

```csharp
string apiBaseUri = "https://api.ipify.org";
string queryContent = string.Empty;
IDictionary<string, string> queryParameters = new Dictionary<string, string>();

ISampleApiToken token = container.Resolve<ISampleApiToken>();
ISampleApiHttpRepository httpRepository = container.Resolve<ISampleApiHttpRepository>();

ISampleApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, apiBaseUri, queryPath, queryParameters, queryContent);

string response = await httpRepository.HttpAsync(queryInformation);

// OR

IResultInformation<string> response = await httpRepository.HttpWithStatusAsync(queryInformation);

```

Complete samples are available in the `Tests.Puffix.Rest` project.
