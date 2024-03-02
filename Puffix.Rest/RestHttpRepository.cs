using System.Text.Json;

namespace Puffix.Rest;

public abstract class RestHttpRepository<QueryInformationContainerT, TokenT> : IRestHttpRepository<QueryInformationContainerT, TokenT>
    where QueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{
    private static readonly Func<HttpContent, Task<string>> extracResultAsString = async (httpContent) => await httpContent.ReadAsStringAsync();
    private static readonly Func<HttpContent, Task<Stream>> extracResultAsStream = async (httpContent) => await httpContent.ReadAsStreamAsync();

    protected readonly IHttpClientFactory httpClientFactory;

    public RestHttpRepository(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public abstract QueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent);

    public abstract QueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent);

    protected static string BuildUri(string apiUri, string queryPath, string queryParameters)
    {
        string builtUri = apiUri.TrimEnd('/');
        builtUri = string.IsNullOrEmpty(queryPath) ? builtUri : $"{builtUri}/{queryPath.TrimStart('/')}";
        builtUri = string.IsNullOrEmpty(queryParameters) ? builtUri : $"{builtUri}?{queryParameters.TrimStart('?')}";

        return builtUri;
    }

    public async Task<string> HttpAsync(QueryInformationContainerT queryInformation)
    {
        return await BaseCallHttpAsync(queryInformation, extracResultAsString);
    }

    public async Task<Stream> HttpStreamAsync(QueryInformationContainerT queryInformation)
    {
        return await BaseCallHttpAsync(queryInformation, extracResultAsStream);
    }

    public async Task<ResultT> HttpJsonAsync<ResultT>(QueryInformationContainerT queryInformation)
    {
        Func<HttpContent, Task<ResultT>> extracJsonResult = async (httpContent) =>
        {
            Stream resultStream = await httpContent.ReadAsStreamAsync();

            ResultT? result = await JsonSerializer.DeserializeAsync<ResultT>(resultStream);

            return result ?? throw new Exception("Error while reading HTTP content or while deserializing.");
        };

        return await BaseCallHttpAsync(queryInformation, extracJsonResult);
    }

    private async Task<BaseResultT> BaseCallHttpAsync<BaseResultT>(QueryInformationContainerT queryInformation, Func<HttpContent, Task<BaseResultT>> extractResult)
    {
        using HttpClient httpClient = httpClientFactory.CreateClient(GetType().FullName ?? string.Empty);

        foreach (string headerKey in queryInformation.Headers.Keys)
        {
            httpClient.DefaultRequestHeaders.Add(headerKey, queryInformation.Headers[headerKey]);
        }

        if (queryInformation.IsHeaderToken)
        {
            foreach (KeyValuePair<string, string> authHeader in queryInformation.GetAuthenticationHeader())
            {
                httpClient.DefaultRequestHeaders.Add(authHeader.Key, authHeader.Value);
            }
        }

        using HttpResponseMessage response = await HttpCall(httpClient, queryInformation);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error {response.StatusCode}: {response.ReasonPhrase} >> {errorContent}");
        }

        return await extractResult(response.Content);
    }

    private async Task<HttpResponseMessage> HttpCall(HttpClient httpClient, QueryInformationContainerT queryInformation)
    {
        Task<HttpResponseMessage> responseMessageTask;
        if (queryInformation.QuerytHttpMethod == HttpMethod.Get)
        {
            responseMessageTask = httpClient.GetAsync(queryInformation.GetUriWithParameters());
        }
        else if (queryInformation.QuerytHttpMethod == HttpMethod.Put)
        {
            responseMessageTask = httpClient.PutAsync(queryInformation.GetUriWithParameters(), queryInformation.GetQueryContent());
        }
        else if (queryInformation.QuerytHttpMethod == HttpMethod.Post)
        {
            responseMessageTask = httpClient.PostAsync(queryInformation.GetUriWithParameters(), queryInformation.GetQueryContent());
        }
        else if (queryInformation.QuerytHttpMethod == HttpMethod.Delete)
        {
            responseMessageTask = httpClient.DeleteAsync(queryInformation.GetUriWithParameters());
        }
        else
            throw new NotImplementedException($"The http method {queryInformation.QuerytHttpMethod} is not implemented");

        return await responseMessageTask;
    }
}