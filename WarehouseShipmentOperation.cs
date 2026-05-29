using System;

namespace MainfreightProject;

// WarehouseShipmentOperation represents a warehouse department workflow.
// It separates depot/storage handling from transport and customer service behaviour.

// This helps becasue Warehouse staff need visibility over where goods are stored or processed.
// This class keeps warehouse responsibility modular instead of interlinking it witht he Program.cs class.
public class WarehouseShipmentOperation : IShipmentOperation
{
    public void ExecuteOperation(Shipment shipment, Staff staffMember)
    {
        Console.WriteLine("Warehouse operation selected.");
        Console.WriteLine("Assigned staff:");
        Console.WriteLine(staffMember.ViewStaffInfo());
        Console.WriteLine();
        Console.WriteLine("Shipment details:");
        Console.WriteLine(shipment.getShipmentInfo());
        Console.WriteLine();
        
    }
}