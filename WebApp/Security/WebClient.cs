using System.Text.Json;
using System.Text;
using Common.Interfaces;
using Common.Result;
using Common.Param;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection.Metadata;
using System;
using Microsoft.JSInterop;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace WebApp.Security
{

    public class WebClient : IWebClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string Token = string.Empty;
        private IJSRuntime _js;
        public WebClient(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper
            };
            _js = js;
        }

        public async Task<TResult> GetAsync<TMethod, TResult>(TMethod endpoint) where TResult : BaseResult where TMethod : Enum
        {
            var url = _httpClient.BaseAddress!.ToString();
            url += $"{endpoint.GetType().Name}/{endpoint.ToString()}";
            return await GetAsync<TResult>(url);
        }

        public async Task<TResult> GetAsync<TResult>(string endpoint) where TResult : BaseResult
        {
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(endpoint),
                
            };
            return await SendRequest<TResult>(requestMessage);
        }

        public async Task<TResult> PostAsync<TParam, TMethod, TResult>(TMethod endpoint, TParam data) where TResult : BaseResult where TMethod : Enum where TParam : BaseParam
        {
            await SetAuthorizations();

            //var url = $"{await GetUrl()}/api/{method.GetType().Name}/{method.ToString()}{query?.GetQueryString()}";
            var url = _httpClient.BaseAddress!.ToString();
            url += $"{endpoint.GetType().Name}/{endpoint.ToString()}";
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = CreateJsonContent(data),
            };

            return await SendRequest<TResult>(requestMessage);
        }

        private StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<TResult> SendRequest<TResult>(HttpRequestMessage requestMessage) where TResult : BaseResult
        {
            try
            {
                requestMessage.SetBrowserRequestCache(BrowserRequestCache.NoCache);
                requestMessage.SetBrowserRequestMode(BrowserRequestMode.Cors);
                requestMessage.SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);

                if (!string.IsNullOrEmpty(Token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                }

                var response = await _httpClient.SendAsync(requestMessage);
                var content = await requestMessage.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TResult>(content);
                return result!;

            }
            catch (Exception ex)
            {
                var baseResult = new BaseResult
                {
                    ResultCode = Common.Type.ResultType.ClientError,
                    Message = "Error desconocido de cliente"
                };
                var json = JsonSerializer.Serialize(baseResult);
                return JsonSerializer.Deserialize<TResult>(json)!;
            }
        }
        private async Task SetAuthorizations()
        {
            var token = await _js.GetItem("Token");
            this.Token = token;
        }
    }
}
