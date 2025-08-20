using Azure;
using Bogus.Bson;
using Microsoft.Extensions.Options;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Services.AttendanceServices.Dto;
using NewAttendanceCalculationAPI.Services.HttpClientServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewAttendanceCalculationAPI.Services.OdooServices
{
      public class OdooPushingDataService : IOdooPushingDataService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientSettings _httpClientSettings;
        private readonly ILogger<OdooPushingDataService> _logger;
        private readonly OdooLoginConfig _loginConfig;
        private readonly ApiEndpoints _apiEndpoints;
        private string? _authToken;
        private DateTime? _tokenExpiration;
        private string? _baseUrl;
        private readonly IOdooTokenService _odooTokenService;

        public OdooPushingDataService(
            HttpClient httpClient,
            IOptions<HttpClientSettings> httpClientSettings,
            IOptions<OdooLoginConfig> loginConfig,
            ILogger<OdooPushingDataService> logger,
            IOdooTokenService odooTokenService,
            IOptions<ApiEndpoints> apiEndpoints
            )
        {
            _httpClient = httpClient;
            _httpClientSettings = httpClientSettings.Value;
            _logger = logger;
            _loginConfig = loginConfig.Value;    // Bind to LoginConfig section
            _baseUrl = _httpClientSettings.BaseAddress;
            _odooTokenService = odooTokenService;
            _apiEndpoints = apiEndpoints.Value;  // Bind to ApiEndpoints section

        }



        public async Task<bool> InsertCalculatedAttendanceAsync(List<AttendanceLogForOdoo> data)
        {
            try
            {
                await _odooTokenService.EnsureTokenAsync();  // Ensure the token is valid before making the request

                var requestPayload = new
                {
                    data
                };

                var json = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_apiEndpoints.MainUrl}{_apiEndpoints.InsertAttendance}";

                var response = await _httpClient.PostAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to push attendance. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while pushing attendance data.");
                throw;
            }
        }
    }
}
