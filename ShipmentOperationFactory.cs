using System;

namespace MainfreightProject;

// This class represents the Factory/Creator role in the Factory Method pattern.


// Program.cs should not directly create every department operation object this gives it too muhc repsoblit wihc means it will be easy to attack or even break.
// If object creation stays inside the staff menu, every new Mainfreight workflow. will require changes to the menu code. 

public class ShipmentOperationFactory
{
    public IShipmentOperation CreateOperation(string operationType)
    {
        switch (operationType.ToLower())
        {
            case "transport":
                return new TransportShipmentOperation();

            case "warehouse":
                return new WarehouseShipmentOperation();

            case "customerservice":
                return new CustomerServiceOperation();

            case "return":
                return new ReturnedGoodsOperation();

            default:
                throw new ArgumentException("Invalid shipment operation type selected.");
        }
    }
}