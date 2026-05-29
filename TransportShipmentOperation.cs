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
        Console.WriteLine("Transport operation selected.");
        Console.WriteLine();
        Console.WriteLine("Assigned staff:");
        Console.WriteLine(staffMember.ViewStaffInfo());
        Console.WriteLine();
        Console.WriteLine("Shipment details:");
        Console.WriteLine(shipment.getShipmentInfo());
        Console.WriteLine();
        
    }
}