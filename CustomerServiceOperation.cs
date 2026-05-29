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
        Console.WriteLine("Customer service operation selected.");
        Console.WriteLine("Assigned staff:");
        Console.WriteLine(staffMember.ViewStaffInfo());
        Console.WriteLine();
        Console.WriteLine("Shipment details:");
        Console.WriteLine(shipment.getShipmentInfo());
        Console.WriteLine();
        
    }
}