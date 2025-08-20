using Azure;
using Azure.Core;
using Microsoft.Extensions.Options;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.HttpClientServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
       public class OdooTokenService:IOdooTokenService
    {

        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        private readonly HttpClientSettings _httpClientSettings;
        private readonly OdooLoginConfig _loginConfig;
        private string? _authToken;
        private DateTime? _tokenExpiration;
        private string? _baseUrl;


        public OdooTokenService(HttpClientSettings httpClientSettings,
            HttpClient httpClient,
            IOptions<ApiEndpoints> apiEndpoints,
            IOptions<OdooLoginConfig> loginConfig)
        {
            _httpClientSettings = httpClientSettings;
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints.Value;  // Bind to ApiEndpoints section
            _loginConfig = loginConfig.Value;    // Bind to LoginConfig section
            _baseUrl = _httpClientSettings.BaseAddress;
        }


        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {

            var fullUrl = $"{_baseUrl}{_apiEndpoints.Login}";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, request);
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }


        public async Task EnsureTokenAsync()
        {
            if (_authToken == null || _tokenExpiration <= DateTime.UtcNow)
            {
                var loginRequest = new LoginRequest
                {
                    Params = new LoginParams
                    {
                        Data = new List<LoginData>
                    {
                        new LoginData
                        {
                            Username = _loginConfig.Username,
                            Password = _loginConfig.Password,
                            Db = _loginConfig.Db
                        }
                    }
                    }
                };

                var loginResponse = await LoginAsync(loginRequest);
                _authToken = loginResponse?.Result?.Token;
                _tokenExpiration = DateTime.UtcNow.AddMinutes(30);  // Assuming the token expires in 30 minutes
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }
    }

}
