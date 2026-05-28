using System;

namespace MainfreightProject;

// StaffStatusNotifier prepares an internal staff-facing status update.
//

// This is a concrete listener in the Observer pattern.


// Staff need visibility when shipment information changes. In a future version,
// this could update an internal staff dashboard or operational alert.
public class StaffStatusNotifier : IShipmentStatusListener
{
    public void OnShipmentStatusChanged(Shipment shipment, string message)
    {
        Console.WriteLine("Staff status update recorded for shipment " + shipment.getShipmentID() + ".");
        Console.WriteLine("Staff message: " + message);
    }
}