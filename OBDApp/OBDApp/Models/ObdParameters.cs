using System;
using System.Collections.Generic;
using System.Text;

namespace OBDApp.Models
{
    public class ObdParameters
    {
        public ObdParameters() {
            dateTime = DateTime.Now;
        }

        public string VIN { get; set; }
        public string Speed { get; set; }
        public string Rpm { get; set; }
        public string fuelSystemStatus { get; set; }
        public string engineCoolantTemperature { get; set; }
        public string fuelPressure { get; set; }
        public string throttlePosition { get; set; }
        public string runTimeSinceEngineStart { get; set; }
        public string distanceTravelledSinceCodeCleared { get; set; }
        public string engineTemperature { get; set; }
        public string massAirflow { get; set; }
        public DateTime dateTime { get; set; }

        public static Dictionary<string, string> createPIDdictionary()
        {

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("Speed", "01 0D\r");
            dictionary.Add("RPM", "01 0C\r");
            dictionary.Add("EngineTemperature", "01 05\r");
            dictionary.Add("throttlePosition", "01 11\r");
            dictionary.Add("massAirflow", "01 10\r");
            dictionary.Add("VIN", "09 02\r");

            return dictionary;
        }
    }
}
