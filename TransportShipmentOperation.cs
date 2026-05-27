using System;

namespace MainfreightProject;

// TransportShipmentOperation represents a transport department workflow.
// It keeps movement/delivery-related logic separate from other staff operations.


// Yhis helps becasue the Transport staff focus on shipment movement, depot transfer, and delivery progress.
// Keeping this in its own class improves cohesion because this class has one focused job it provides better cohesion
public class TransportShipmentOperation : IShipmentOperation
{
    public void ExecuteOperation(Shipment shipment, Staff staffMember)
    {
        Console.WriteLine("\n=== Factory Method Demonstration ===");
        Console.WriteLine("Factory created: TransportShipmentOperation");
        Console.WriteLine("Executed through: IShipmentOperation");
        Console.WriteLine("\n--- Transport Shipment Operation ---");
        Console.WriteLine("Purpose: supports movement tracking, depot transfer, and delivery coordination.");

        Console.WriteLine("\nHandled by:");
        Console.WriteLine(staffMember.ViewStaffInfo());

        Console.WriteLine("\nShipment involved:");
        Console.WriteLine(shipment.getShipmentInfo());
    }
}