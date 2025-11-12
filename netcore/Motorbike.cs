namespace congestion.calculator
{
    public sealed class Motorbike : IVehicle
    {
        public VehicleType Type
        {
            get
            {
                return VehicleType.Motorcycle;
            }
        }
    }
}