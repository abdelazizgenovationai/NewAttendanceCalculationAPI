namespace NewAttendanceCalculationAPI.Services.HttpClientServices
{
       public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, HttpContent content);
    }
}
