using prague_parking_v2.pragueParkingV2.Core.Models;
using prague_parking_v2.pragueParkingV2.DataAccess;

namespace prague_parking_v2.pragueParkingV2.Core.Services
{
    public class ParkingGarage
    {

        // set new instance of spots
        private List<ParkingSpot> parkingSpots = new List<ParkingSpot>();
        private readonly JsonDataAccess _dataAccess;
        private Configuration _config;

        public ParkingGarage(JsonDataAccess dataAccess, Configuration config)
        {
            //set values
            _dataAccess = dataAccess;
            _config = config;
            InitializeParkingSpots();
        }

        private void InitializeParkingSpots()
        {
            parkingSpots = _dataAccess.LoadParkingData();

            // If there's no saved data, or the number of spots doesn't match the config, recreate the spots
            if (parkingSpots.Count == 0 || parkingSpots.Count != _config.ParkingSpotCount)
            {
                parkingSpots.Clear(); // Clear existing spots if any
                for (int i = 1; i <= _config.ParkingSpotCount; i++)
                {
                    parkingSpots.Add(new ParkingSpot(i));
                }
                _dataAccess.SaveParkingData(parkingSpots); // Save the newly created spots
            }
            System.Console.WriteLine($"Initialized {parkingSpots.Count} parking spots.");
        }

        public bool ParkVehicle(Vehicle vehicle)
        {
            var availableSpot = parkingSpots.FirstOrDefault(spot => spot.CanPark(vehicle));

            //validate
            if (availableSpot != null)
            {
                vehicle.ParkedTime = DateTime.Now;
                availableSpot.ParkVehicle(vehicle);
                SaveParkingData();
                return true;
            }
            return false;
        }

        // This method removes a vehicle from the parking lot and calculates the fee.
        // It returns the removed vehicle and the parking fee.
        public Vehicle? RemoveVehicle(string registrationNumber)
        {
            foreach (var spot in parkingSpots)
            {
                var removedVehicle = spot.RemoveVehicle(registrationNumber);

                if (removedVehicle != null)
                {
                    SaveParkingData();
                    return removedVehicle;
                }
            }
            // If no vehicle was found with the given registration number, return null and a fee of 0.
            return null;
        }

        // Searches for and returns the vehicle with the specified registration number.
        public Vehicle? FindVehicle(string registrationNumber)
        {
            return parkingSpots
                .Where(spot => spot.IsOccupied)
                .SelectMany(spot => spot.GetParkedVehicles())
                .FirstOrDefault(v => v.RegistrationNumber == registrationNumber);
        }


        public List<ParkingSpot> GetParkingSpots()
        {
            return parkingSpots;
        }

        public void MoveVehicle(string registrationNumber, int newSpotNumber)
        {
            ParkingSpot originalSpot = null;
            Vehicle vehicle = null;

            // Find the original spot and remove the vehicle
            foreach (var spot in parkingSpots)
            {
                vehicle = spot.RemoveVehicle(registrationNumber);
                if (vehicle != null)
                {
                    originalSpot = spot;
                    break;
                }
            }

            //validate
            if (vehicle != null && originalSpot != null)
            {
                var newSpot = parkingSpots.FirstOrDefault(spot => spot.Number == newSpotNumber);
                if (newSpot != null && newSpot.CanPark(vehicle))
                {
                    newSpot.ParkVehicle(vehicle);
                    SaveParkingData();
                }
                else
                {
                    // If we can't park in the new spot, put it back in the original spot
                    originalSpot.ParkVehicle(vehicle);
                    SaveParkingData();
                    throw new InvalidOperationException("Cannot move vehicle to the specified spot.");
                }
            }
            else
            {
                throw new InvalidOperationException("Vehicle not found.");
            }
        }

        private void SaveParkingData()
        {
            _dataAccess.SaveParkingData(parkingSpots);
        }

    }
}
