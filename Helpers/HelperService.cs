using AutoMapper;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Models;
using System.Text.Json;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;

namespace NewAttendanceCalculationAPI.Helpers
{
    public class HelperService
    {
        private readonly HRSystemServiceContext _context;
        private readonly IMapper _mapper;


        public HelperService(HRSystemServiceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task ReadAndInsertBiometricEventsAsync()
        {
            string filePath = @"C:\Users\ThinkPad\Desktop\filtered-response-23-3-2025-22-3-2025.json";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            try
            {
                // Read JSON file
                string jsonData = await File.ReadAllTextAsync(filePath);

                // Deserialize JSON
                var biometricEvents = JsonSerializer.Deserialize<List<BiometricEventDto>>(jsonData);

                if (biometricEvents != null && biometricEvents.Count > 0)
                {

                    var toInsert = _mapper.Map<List<BiometricEvent>>(biometricEvents);

                    // Insert into the database
                    await _context.BiometricEvents.AddRangeAsync(toInsert);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Data inserted successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
