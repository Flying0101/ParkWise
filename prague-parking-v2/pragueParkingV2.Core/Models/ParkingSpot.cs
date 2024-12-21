using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prague_parking_v2.pragueParkingV2.Core.Models
{
    public class ParkingSpot
    {
        // Gets or sets the unique identifier number for this parking spot.
        public int Number { get; set; }

        // Private list to store vehicles currently parked in this spot.
        private List<Vehicle> parkedVehicles = new List<Vehicle>();

        // Gets or sets the timestamp when the first vehicle was parked in this spot.
        public DateTime? ParkedTime { get; set; }

        public List<VehicleInfo> ParkedVehiclesInfo
        {
            get => parkedVehicles.Select(v => new VehicleInfo(v)).ToList();
            set => parkedVehicles = value.Select(info => info.ToVehicle()).ToList();
        }

        public ParkingSpot(int number)
        {
            Number = number;
        }

        public bool IsOccupied => parkedVehicles.Any();

        //Determines if a vehicle can be parked in this spot based on current occupancy
        public bool CanPark(Vehicle vehicle)
        {
            if (!IsOccupied) return true;
            if (parkedVehicles.Count == 1 && parkedVehicles[0] is Motorcycle && vehicle is Motorcycle)
            {
                return true; // Two motorcycles can share a spot
            }
            return false;
        }

        // Parks a vehicle in this spot if space is available.
        public void ParkVehicle(Vehicle vehicle)
        {
            //validate
            if (CanPark(vehicle))
            {
                parkedVehicles.Add(vehicle);
                ParkedTime = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationException("Cannot park the vehicle in this spot.");
            }
        }

        // Removes a vehicle from this spot based on its registration number.
        public Vehicle? RemoveVehicle(string registrationNumber)
        {
            var vehicle = parkedVehicles.FirstOrDefault(v => v.RegistrationNumber == registrationNumber);
            if (vehicle != null)
            {
                parkedVehicles.Remove(vehicle);
                if (!IsOccupied)
                {
                    ParkedTime = null;
                }
                return vehicle;
            }
            return null;
        }

        public List<Vehicle> GetParkedVehicles()
        {
            return new List<Vehicle>(parkedVehicles);
        }

        // Provides a string representation of the parking spot's current state.
        // Format varies based on occupancy and vehicle types.
        public override string ToString()
        {
            if (!IsOccupied) return $"[EMPTY:     ]";
            if (parkedVehicles.Count == 1)
            {
                var vehicle = parkedVehicles[0];
                if (vehicle is Car) return $"[CAR:{vehicle.RegistrationNumber,5}]";
                if (vehicle is Motorcycle) return $"[MC:{vehicle.RegistrationNumber,3}]";
            }
            else if (parkedVehicles.Count == 2)
            {
                return $"[MC:{parkedVehicles[0].RegistrationNumber,1} + {parkedVehicles[1].RegistrationNumber,1}]";
            }
            return $"[{Number:D2}:ERROR]";
        }

        //testing, change comment later
        public class VehicleInfo
        {
            public string RegistrationNumber { get; set; }
            public string VehicleType { get; set; }
            public DateTime ParkedTime { get; set; }

            public VehicleInfo() {} 
            public VehicleInfo(Vehicle vehicle)
            {
                RegistrationNumber = vehicle.RegistrationNumber;
                VehicleType = vehicle is Car ? "Car" : "Motorcycle";
                ParkedTime = vehicle.ParkedTime;
            }

            // Converts the current instance of a Vehicle to a specific Vehicle type (Car or Motorcycle)
            public Vehicle ToVehicle()
            {
                return VehicleType == "Car"
                    ? new Car(RegistrationNumber) { ParkedTime = ParkedTime }
                    : new Motorcycle(RegistrationNumber) { ParkedTime = ParkedTime };
            }
        }
    }
}
