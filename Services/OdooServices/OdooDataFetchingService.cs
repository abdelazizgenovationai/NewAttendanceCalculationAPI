using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

using System.Net.Http.Headers;

using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.HttpClientServices.Dto;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Helpers.Dto;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
 public class OdooDataFetchingService: IOdooDataFetchingService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        private readonly HttpClientSettings _httpClientSettings;
        private readonly OdooLoginConfig _loginConfig;
        private readonly BiometricApiConfig _biometricConfig;
        private string? _authToken;
        private DateTime? _tokenExpiration;
        private string? _baseUrl;

        public OdooDataFetchingService(
            HttpClientSettings httpClientSettings, 
            HttpClient httpClient, 
            IOptions<ApiEndpoints> apiEndpoints, 
            IOptions<OdooLoginConfig> loginConfig, 
            IOptions<BiometricApiConfig> biometricConfig)
        {
            _httpClientSettings = httpClientSettings;
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints.Value;  // Bind to ApiEndpoints section
            _loginConfig = loginConfig.Value;    // Bind to LoginConfig section
            _biometricConfig = biometricConfig.Value;
            _baseUrl = _httpClientSettings.BaseAddress;
        }

        private async Task EnsureTokenAsync()
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



        public async Task<GetHolidayResponse?> GetHolidayAsync(GetHolidayRequest request)
        {
            await EnsureTokenAsync();  // Ensure the token is valid before making the request
            var url = QueryHelpers.AddQueryString($"{_baseUrl}{_apiEndpoints.GetHoliday}", new Dictionary<string, string?>
            {
                ["DateFrom"] = request.DateFrom,
                ["DateTo"] = request.DateTo,
                ["Name"] = request.Name
            });

            var response = await _httpClient.GetAsync(url);
            var result= await response.Content.ReadFromJsonAsync<GetHolidayResponse>();
           
            return result; 
        }

        public async Task<GetCalendarResponse?> GetCalendarAsync(GetCalendarRequest request)
        {
            // Ensure token is valid before making the request
            await EnsureTokenAsync();

            // Construct the URL with query string parameters
            var url = QueryHelpers.AddQueryString($"{_baseUrl}{_apiEndpoints.GetCalendar}", new Dictionary<string, string?>
            {
                ["EmployeeId"] = request.EmployeeId.ToString(),
                ["DateFrom"] = request.DateFrom,
                ["DateTo"] = request.DateTo
            });

            // Send the GET request with the query string
            var response = await _httpClient.GetAsync(url);

            // Deserialize and return the response
            return await response.Content.ReadFromJsonAsync<GetCalendarResponse>();
        }


        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {

            var fullUrl = $"{_baseUrl}{_apiEndpoints.Login}";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, request);
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<GetAttendanceResponse?> GetAttendanceAsync (GetAttendanceRequest request)
        {
            await EnsureTokenAsync();  // Ensure the token is valid before making the request
            var url = QueryHelpers.AddQueryString($"{_baseUrl}{_apiEndpoints.GetAttendance}", new Dictionary<string, string?>
            {
                ["DateFrom"] = request.DateFrom,
                ["DateTo"] = request.DateTo,
                ["EmployeeId"] = request.EmployeeId.ToString(),
            });

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<GetAttendanceResponse>();

            return result;
        }

        public async Task<OdooEmployeeResponse?> GetEmployeeFromOdooAsync(OdooEmployeeRequest request)
        {
            await EnsureTokenAsync();  // Reuse your existing token management

            var url = QueryHelpers.AddQueryString($"{_biometricConfig.BaseAddress}/zts/get/EmployeesShifts",
                new Dictionary<string, string?>
                {
                    ["EmployeeId"] = request.EmployeeId?.ToString(),
                    ["DateFrom"] = request.DateFrom,
                    ["DateTo"] = request.DateTo
                });

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OdooEmployeeResponse>();
        }
    }
}
