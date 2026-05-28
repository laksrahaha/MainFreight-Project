using System;

namespace MainfreightProject;

// CustomerStatusNotifier prepares a customer facing status update.
//

// This is a concrete listener in the Observer pattern whihc is one of the two design patterns that were used.
//
// Customers rely on accurate shipment visibility
public class CustomerStatusNotifier : IShipmentStatusListener
{
    public void OnShipmentStatusChanged(Shipment shipment, string message)
    {
        Console.WriteLine("Customer status update prepared for shipment " + shipment.getShipmentID() + ".");
        Console.WriteLine("Customer message: " + message);
    }
}