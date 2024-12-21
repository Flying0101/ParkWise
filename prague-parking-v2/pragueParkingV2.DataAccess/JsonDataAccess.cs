using prague_parking_v2.pragueParkingV2.Core.Models;
using System.Text.Json;
using pragueParkingFilePathsV2;

namespace prague_parking_v2.pragueParkingV2.DataAccess
{
    public class JsonDataAccess
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataAccess()
        {
           
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new VehicleConverter() }
            };
        }
       
        // Loads parking data from the JSON file.
        public List<ParkingSpot> LoadParkingData()
        {
            string json = JsonPaths.LoadParkData();

            if (json != null)
            {
                return JsonSerializer.Deserialize<List<ParkingSpot>>(json, _jsonOptions) ?? new List<ParkingSpot>();
            }
            return new List<ParkingSpot>();
        }

        // Saves the provided list of ParkingSpot to the JSON file.
        public void SaveParkingData(List<ParkingSpot> parkingSpots)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(parkingSpots, new JsonSerializerOptions { WriteIndented = true });
                JsonPaths.SaveParkData(jsonString);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error saving parking data: {ex.Message}");
                System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Loads configuration data from the JSON file. Returns a Configuration object.
        public Configuration LoadConfiguration()
        {

            string json = JsonPaths.LoadConfigData();

            if (json != null)
            {
                return JsonSerializer.Deserialize<Configuration>(json) ?? new Configuration();
            }
            else
            {
                System.Console.WriteLine("Config file not found");
                System.Console.WriteLine("loading new default configuration file...");
                return new Configuration();
            }
        }

        // Saves the provided Configuration object to the JSON file.
        public void SaveConfiguration(Configuration config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            JsonPaths.SaveConfig(json);
        }
    }
}
