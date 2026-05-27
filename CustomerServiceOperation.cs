using System;

namespace MainfreightProject;

// CustomerServiceOperation represents the customer facing workflow
// It separates customer enquiry/status explanation from transport and warehouse logic
//
// Why this helps Mainfreight is becasue the 
// Customer service staff need to explain shipment information clearly to customers otherwise it causes distrubances
// Keeping this separate supports clearer responsibility allocation so that encasplation can be maintained and the system is easier to extend later if needed.
public class CustomerServiceOperation : IShipmentOperation
{
    public void ExecuteOperation(Shipment shipment, Staff staffMember)
    {
        Console.WriteLine("\n=== Factory Method Demonstration ===");
        Console.WriteLine("Factory created: CustomerServiceOperation");
        Console.WriteLine("Executed through: IShipmentOperation");
        Console.WriteLine("\n--- Customer Service Operation ---");
        Console.WriteLine("Purpose: supports customer enquiry handling and shipment status explanation.");

        Console.WriteLine("\nHandled by:");
        Console.WriteLine(staffMember.ViewStaffInfo());

        Console.WriteLine("\nShipment involved:");
        Console.WriteLine(shipment.getShipmentInfo());
    }
}