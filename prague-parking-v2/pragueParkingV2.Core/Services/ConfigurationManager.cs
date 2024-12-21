using prague_parking_v2.pragueParkingV2.DataAccess;

namespace prague_parking_v2.pragueParkingV2.Core.Services
{
    // Class responsible for managing application configuration settings.
    public class ConfigurationManager
    {
        private readonly JsonDataAccess _dataAccess;
        public Configuration CurrentConfig { get; private set; }

        // Constructor that initializes the ConfigurationManager with a JsonDataAccess instance
        public ConfigurationManager(JsonDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            CurrentConfig = _dataAccess.LoadConfiguration();
        }

        // Saves the current configuration to the data source.
        public void SaveConfiguration()
        {
            _dataAccess.SaveConfiguration(CurrentConfig);
        }

        // Reloads the configuration from the data source, updating CurrentConfig
        public void ReloadConfiguration()
        {
            CurrentConfig = _dataAccess.LoadConfiguration();

        }

        // Updates the parking spot count in the current configuration and saves it
        public void UpdateParkingSpotCount(int count)
        {
            CurrentConfig.ParkingSpotCount = count;
            SaveConfiguration();
        }


        // Updates the hourly prices for cars and motorcycles in the current configuration and saves it
        public void UpdatePrices(decimal carPrice, decimal motorcyclePrice)
        {
            CurrentConfig.CarPricePerHour = carPrice;
            CurrentConfig.MotorcyclePricePerHour = motorcyclePrice;
            SaveConfiguration();
        }
    }
}


// Class representing the configuration settings for the parking system.
public class Configuration
{
    public int ParkingSpotCount { get; set; } = 100;
    public decimal CarPricePerHour { get; set; } = 20;
    public decimal MotorcyclePricePerHour { get; set; } = 10;
    // Add other configuration properties later.
}