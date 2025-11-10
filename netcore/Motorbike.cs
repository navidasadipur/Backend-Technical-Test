
namespace congestion.calculator
{
    public class Motorcycle : IVehicle
    {
        VehicleType IVehicle.GetVehicleType()
        {
            return VehicleType.Motorcycle;
        }
    }
}