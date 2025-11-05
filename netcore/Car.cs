namespace congestion.calculator
{
    public sealed class Car : IVehicle
    {
        public VehicleType Type
        {
            get
            {
                return VehicleType.Car;
            }
        }
    }
}