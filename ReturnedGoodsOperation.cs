using System;

namespace MainfreightProject;

// ReturnedGoodsOperation represents reverse logistics and return processing
// Returned goods are part of the refined system scope, so this class keeps
// return processing separate from normal delivery handling
//

// If return logic grows later, it can be extended here instead of changing whihc supports the open to extension closed for modification
// transport, warehouse, or menu code.
public class ReturnedGoodsOperation : IShipmentOperation
{
    public void ExecuteOperation(Shipment shipment, Staff staffMember)
    {
        Console.WriteLine("Returned goods operation selected.");
        Console.WriteLine("Assigned staff:");
        Console.WriteLine(staffMember.ViewStaffInfo());
        Console.WriteLine();
        Console.WriteLine("Shipment details:");
        Console.WriteLine(shipment.getShipmentInfo());
        Console.WriteLine();
        
    }
}