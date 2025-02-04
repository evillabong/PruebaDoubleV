using System.Text.Json;
using System.Text;
using Common.Interfaces;
using Common.Result;
using Common.Param;
using System.Text.Json.Serialization;

namespace WebApp.Security
{

    public class WebClient : IWebClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private ILogger _logger;
        public WebClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }

        public async Task<TResult> GetAsync<TResult, TMethod>(string endpoint) where TResult : BaseResult where TMethod : Enum
        {
            var response = await _httpClient.GetAsync($"{typeof(TMethod)}/{nameof(TMethod)}");
            response.EnsureSuccessStatusCode();
            return await DeserializeResponse<TResult>(response);
        }

        public async Task<TResult> PostAsync<TResult, TMethod, TParam>(TMethod endpoint, TParam data) where TResult : BaseResult where TMethod : Enum where TParam : BaseParam
        {
            var content = CreateJsonContent(data);
            var response = await _httpClient.PostAsync($"{typeof(TMethod)}/{nameof(TMethod)}", content);
            response.EnsureSuccessStatusCode();
            return await DeserializeResponse<TResult>(response);
        }

        private StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<TResult> DeserializeResponse<TResult>(HttpResponseMessage response) where TResult : BaseResult
        {
            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResult>(responseContent, _jsonOptions)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, this.GetType().Name);
                var @base = new BaseResult
                {
                    ResultCode = Common.Type.ResultType.UnknowError,
                    Message = "Error desconocido"
                };
                var json = JsonSerializer.Serialize(@base);
                var baseResult = System.Text.Json.JsonSerializer.Deserialize<TResult>(json)!;

                return baseResult;
            }
        }
    }
}
