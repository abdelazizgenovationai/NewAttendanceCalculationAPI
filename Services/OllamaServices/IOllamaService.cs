namespace NewAttendanceCalculationAPI.Services.OllamaServices
{
    public interface IOllamaService
    {
        public Task<string> AskModelAsync(string prompt);
    }
}
