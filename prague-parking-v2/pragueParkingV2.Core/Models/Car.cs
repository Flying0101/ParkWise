namespace prague_parking_v2.pragueParkingV2.Core.Models
{
    // Represents a Car vehicle, inheriting from the Vehicle base class.
    public class Car : Vehicle
    {
        public Car(string registrationNumber) : base(registrationNumber) { }
        public override int Size => 4;
    }
}
