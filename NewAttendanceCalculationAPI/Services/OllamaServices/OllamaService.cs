using NewAttendanceCalculationAPI.Services.OllamaServices.Dto;
using NewAttendanceCalculationAPI.Services.OllamaServices.Dto;

namespace NewAttendanceCalculationAPI.Services.OllamaServices
{
    public class OllamaService:IOllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:11434/");
        }

        public async Task<string> AskModelAsync(string prompt)
        {

            try
            {

            var request = new
            {
                model = "llama3",
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("api/generate", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponseDto>();
            return result?.Response ?? string.Empty;


            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
