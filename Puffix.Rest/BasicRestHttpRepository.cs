using System.Text.Json;

namespace Puffix.Rest;

public abstract class BasicRestHttpRepository<BasicQueryInformationContainerT, TokenT> : IBasicRestHttpRepository<BasicQueryInformationContainerT, TokenT>
    where BasicQueryInformationContainerT : IBasicQueryInformation<TokenT>
    where TokenT : IToken
{
    private static readonly Func<HttpContent, Task<string>> extracResultAsString = async (httpContent) => await httpContent.ReadAsStringAsync();
    private static readonly Func<HttpContent, Task<Stream>> extracResultAsStream = async (httpContent) => await httpContent.ReadAsStreamAsync();

    protected readonly IHttpClientFactory httpClientFactory;

    public BasicRestHttpRepository(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public abstract BasicQueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    public abstract BasicQueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    public abstract BasicQueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    public abstract BasicQueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    protected static string BuildUri(string apiUri, string queryPath, string queryParameters)
    {
        string builtUri = apiUri.TrimEnd('/');
        builtUri = string.IsNullOrEmpty(queryPath) ? builtUri : $"{builtUri}/{queryPath.TrimStart('/')}";
        builtUri = string.IsNullOrEmpty(queryParameters) ? builtUri : $"{builtUri}?{queryParameters.TrimStart('?')}";

        return builtUri;
    }

    public async Task<string> HttpAsync(BasicQueryInformationContainerT queryInformation)
    {
        IResultInformation<string> httpCallResult = await BaseCallHttpAsync(queryInformation, extracResultAsString, true);
        return httpCallResult.ResultContent!;
    }

    public async Task<Stream> HttpStreamAsync(BasicQueryInformationContainerT queryInformation)
    {
        IResultInformation<Stream> httpCallResult = await BaseCallHttpAsync(queryInformation, extracResultAsStream, true);
        return httpCallResult.ResultContent!;
    }

    public async Task<ResultT> HttpJsonAsync<ResultT>(BasicQueryInformationContainerT queryInformation)
    {
        IResultInformation<ResultT> httpCallResult = await BaseHttpJsonAsync<ResultT>(queryInformation, true);
        return httpCallResult.ResultContent!;
    }

    public async Task<IResultInformation<string>> HttpWithStatusAsync(BasicQueryInformationContainerT queryInformation)
    {
        return await BaseCallHttpAsync(queryInformation, extracResultAsString, false);
    }

    public async Task<IResultInformation<Stream>> HttpStreamWithStatusAsync(BasicQueryInformationContainerT queryInformation)
    {
        return await BaseCallHttpAsync(queryInformation, extracResultAsStream, false);
    }

    public async Task<IResultInformation<ResultT>> HttpJsonWithStatusAsync<ResultT>(BasicQueryInformationContainerT queryInformation)
    {
        return await BaseHttpJsonAsync<ResultT>(queryInformation, false);
    }

    private async Task<IResultInformation<ResultT>> BaseHttpJsonAsync<ResultT>(BasicQueryInformationContainerT queryInformation, bool sendErrorOnNotSuccessCode)
    {
        Func<HttpContent, Task<ResultT>> extracJsonResult = async (httpContent) =>
        {
            Stream resultStream = await httpContent.ReadAsStreamAsync();

            ResultT? result = await JsonSerializer.DeserializeAsync<ResultT>(resultStream);

            return result ?? throw new Exception("Error while reading HTTP content or while deserializing.");
        };

        return await BaseCallHttpAsync(queryInformation, extracJsonResult, sendErrorOnNotSuccessCode);
    }

    private async Task<IResultInformation<ResultT>> BaseCallHttpAsync<ResultT>(BasicQueryInformationContainerT queryInformation, Func<HttpContent, Task<ResultT>> extractResult, bool sendErrorOnNotSuccessCode)
    {
        using HttpClient httpClient = httpClientFactory.CreateClient(GetType().FullName ?? string.Empty);

        foreach (KeyValuePair<string, IEnumerable<string>> header in queryInformation.Headers)
        {
            if (header.Value.Count() <= 1)
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.FirstOrDefault());
            else
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        if (queryInformation.IsHeaderToken)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> authHeader in queryInformation.GetAuthenticationHeader())
            {
                if (authHeader.Value.Count() <= 1)
                    httpClient.DefaultRequestHeaders.Add(authHeader.Key, authHeader.Value.FirstOrDefault());
                else
                    httpClient.DefaultRequestHeaders.Add(authHeader.Key, authHeader.Value);
            }
        }

        using HttpResponseMessage response = await HttpCall(httpClient, queryInformation);

        IResultInformation<ResultT> httpCallResult;
        if (!response.IsSuccessStatusCode)
        {
            if (sendErrorOnNotSuccessCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {response.StatusCode}: {response.ReasonPhrase} >> {errorContent}");
            }
            else
            {
                httpCallResult = new ResultInformation<ResultT>(
                    ResultInformation<ResultT>.ExtractHttpHeaders(response.Headers),
                    response.StatusCode,
                    default,
                    false,
                    await response.Content.ReadAsStringAsync()
                );
            }
        }
        else
        {
            httpCallResult = new ResultInformation<ResultT>(
                ResultInformation<ResultT>.ExtractHttpHeaders(response.Headers),
                response.StatusCode,
                await extractResult(response.Content),
                true,
                string.Empty
            );
        }

        return httpCallResult;
    }

    private async Task<HttpResponseMessage> HttpCall(HttpClient httpClient, BasicQueryInformationContainerT queryInformation)
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