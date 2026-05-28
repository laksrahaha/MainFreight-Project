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
        Console.WriteLine("\n=== Factory Method Demonstration ===");
        Console.WriteLine("Factory created: WarehouseShipmentOperation");
        Console.WriteLine("Executed through: IShipmentOperation");
        Console.WriteLine("\n--- Warehouse Shipment Operation ---");
        Console.WriteLine("Purpose: supports depot handling, storage visibility, and warehouse processing.");

        Console.WriteLine("\nHandled by:");
        Console.WriteLine(staffMember.ViewStaffInfo());

        Console.WriteLine("\nShipment involved:");
        Console.WriteLine(shipment.getShipmentInfo());
    }
}