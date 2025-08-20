namespace NewAttendanceCalculationAPI.Services.OdooServices.Dto
{
    using System.Text.Json.Serialization;

  public class LoginResponse
    {
        public string Jsonrpc { get; set; }
        public object Id { get; set; }
        public LoginResult Result { get; set; }

    }

}
