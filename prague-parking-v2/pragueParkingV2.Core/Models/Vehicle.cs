using prague_parking_v2.pragueParkingV2.Core.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace prague_parking_v2.pragueParkingV2.Core.Models
{
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; }
        public DateTime ParkedTime { get; set; }

        // Constructor for the Vehicle class that initializes a vehicle with a registration number.
        protected Vehicle(string registrationNumber)
        {
            //validate
            if (!IsValidRegistrationNumber(registrationNumber))
            {
                throw new ArgumentException("Invalid registration number. It must be 1-10 characters long.");
            }
            RegistrationNumber = registrationNumber;
            ParkedTime = DateTime.Now;
        }

        //validation, Checks if the provided registration number is valid.
        public static bool IsValidRegistrationNumber(string registrationNumber)
        {
            return !string.IsNullOrEmpty(registrationNumber) && registrationNumber.Length <= 10;
        }

        public abstract int Size { get; }
       
    }

    public class VehicleConverter : JsonConverter<Vehicle>
    {
        // Reads and converts JSON data into a Vehicle object, identifying if it's a Car or Motorcycle.
        public override Vehicle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                string type = root.GetProperty("VehicleType").GetString();
                string regNumber = root.GetProperty("RegistrationNumber").GetString();
                DateTime parkedTime = root.GetProperty("ParkedTime").GetDateTime();

                Vehicle vehicle = type == "Car" ? new Car(regNumber) : new Motorcycle(regNumber);
                vehicle.ParkedTime = parkedTime;
                return vehicle;
            }
        }

        public override void Write(Utf8JsonWriter writer, Vehicle value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("RegistrationNumber", value.RegistrationNumber);
            writer.WriteString("VehicleType", value is Car ? "Car" : "Motorcycle");
            writer.WriteString("ParkedTime", value.ParkedTime);
            writer.WriteEndObject();
        }
    }


}
