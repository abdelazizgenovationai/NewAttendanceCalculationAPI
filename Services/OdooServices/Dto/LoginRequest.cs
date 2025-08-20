namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
   using System.Collections.Generic;
using System.Text.Json.Serialization;

 public class LoginRequest
    {
        public LoginParams Params { get; set; }

    }
    public class LoginParams
    {
        public List<LoginData> Data { get; set; }
    }

    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Db { get; set; }
    }

}
