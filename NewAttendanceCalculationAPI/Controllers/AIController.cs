
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Services.OllamaServices;

namespace NewAttendanceCalculationAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly HRSystemServiceContext _context;
        private readonly OllamaService _ollama;

        public AIController(HRSystemServiceContext context, OllamaService ollama)
        {
            _context = context;
            _ollama = ollama;
        }

        [HttpPost("AskMe")]
        public async Task<IActionResult> QueryDatabase([FromBody] string question)
        {
            string prompt = BuildPrompt(question);
            string rawResponse = await _ollama.AskModelAsync(prompt);

            // ✅ Extract only the SQL part
            string sqlQuery = ExtractSqlFromResponse(rawResponse);

            if (!sqlQuery.Trim().ToLower().StartsWith("select"))
                return BadRequest(new { error = "Only SELECT queries are allowed", sqlQuery });

            try
            {
                var results = await _context.BiometricEvents
                    .FromSqlRaw(sqlQuery)
                    .ToListAsync();

                return Ok(new { query = sqlQuery, data = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, sqlQuery });
            }
        }

        private string BuildPrompt(string userQuestion)
        {
            return @$"
You are a SQL Server expert. Convert the following natural language request into a valid SQL Server SELECT query.

Tables:
- BiometricEvents(Id, UserId, Username, EDate, ETime, Access_allowed, DoorControllerId)


User Question: {userQuestion}

Note:
User Question: {userQuestion}

Rules:
- Table: BiometricEvents(Id, UserId, Username, EDate, ETime, Access_allowed, DoorControllerId, EntryExitType).
- Always SELECT all columns listed above and be sure EntryExitType is included, never partial.
- Always include 'Id'.
- Only one clean SELECT query, ending with ';'.
- No multiple SELECTs, no second queries.
- No markdown, no explanations, no formatting.
- No tuple-style WHERE (col1, col2) = (...); use ORDER BY col1 DESC, col2 DESC with TOP 1 instead.
- No duplicate UserIds in the result.
";
        }



        private string ExtractSqlFromResponse(string response)
        {
            var start = response.IndexOf("```sql", StringComparison.OrdinalIgnoreCase);
            var end = response.IndexOf("```", start + 1, StringComparison.OrdinalIgnoreCase);

            if (start != -1 && end != -1)
            {
                return response.Substring(start + 6, end - (start + 6)).Trim();
            }

            // Fallback: Try to find first "SELECT" statement and extract until semicolon
            var selectIndex = response.IndexOf("select", StringComparison.OrdinalIgnoreCase);
            if (selectIndex != -1)
            {
                int endSemicolon = response.IndexOf(";", selectIndex);
                return endSemicolon != -1
                    ? response.Substring(selectIndex, endSemicolon - selectIndex + 1).Trim()
                    : response.Substring(selectIndex).Trim();
            }

            return response.Trim(); // fallback if nothing else worked
        }


    }



}
