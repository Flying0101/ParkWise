namespace prague_parking_v2.pragueParkingV2.Core.Models
{

    // Represents a motorcycle vehicle, inheriting from the Vehicle base class.
    public class Motorcycle : Vehicle
    {
        public Motorcycle(string registrationNumber) : base(registrationNumber) { }
        public override int Size => 2;
    }
}
