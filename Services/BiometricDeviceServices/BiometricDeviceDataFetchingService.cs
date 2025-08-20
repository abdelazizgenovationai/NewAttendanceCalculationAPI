﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewAttendanceCalculationAPI.Services.BiometricDeviceServices
{
    public class BiometricDeviceDataFetchingService: IBiometricDeviceDataFetchingService
    {
        private readonly HttpClient _httpClient;
        private readonly BiometricApiConfig _biometricConfig;
        private readonly ILogger<BiometricDeviceDataFetchingService> _logger;

        public BiometricDeviceDataFetchingService(
            HttpClient httpClient,
            IOptions<BiometricApiConfig> biometricConfig,
            ILogger<BiometricDeviceDataFetchingService> logger)
        {
            _httpClient = httpClient;
            _biometricConfig = biometricConfig.Value;
            _logger = logger;

            var authHeader = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{_biometricConfig.Username}:{_biometricConfig.Password}")));

            _httpClient.DefaultRequestHeaders.Authorization = authHeader;
        }

        public async Task<List<BiometricEventDto>> GetBiometricEventsByDateAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string fromDateString = fromDate.ToString("ddMMyyyy");
                string toDateString = toDate.ToString("ddMMyyyy");


                string dateRange = $"{fromDateString}000000-{toDateString}235959";

                var url = QueryHelpers.AddQueryString(
                    $"{_biometricConfig.BaseAddress}/api.svc/v2/event-ta-date",
                    new Dictionary<string, string?>
                    {
                        ["action"] = "get",
                        ["date-range"] = dateRange,
                        ["field-name"] = "USERID,USERNAME,EDATE,ETIME,ENTRYEXITTYPE,DOORCONTROLLERID,ACCESS_ALLOWED",
                        ["format"] = "json"
                    });

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BiometricDeviceDto>(content);

                return result?.BiometricEventsList ?? new List<BiometricEventDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching biometric events for date {Date}", fromDate.ToString("yyyy-MM-dd") +"-"+ toDate.ToString("yyyy-MM-dd"));
                throw;
            }
        }

     
    }
}
