using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace NewAttendanceCalculationAPI.Models
{
    using System.Text.Json.Serialization;

    public class BiometricEvent
    {

        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

  
        public string Username { get; set; }

 
        public string EDate { get; set; }


        public string ETime { get; set; }

     
        public int EntryExitType { get; set; }


        public int Access_allowed { get; set; }

        public int DoorControllerId { get; set; }
    }

}
