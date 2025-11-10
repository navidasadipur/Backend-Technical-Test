namespace congestion.calculator
{
    public class Car : IVehicle
    {
        VehicleType IVehicle.GetVehicleType()
        {
            return VehicleType.Car;
        }
    }
}