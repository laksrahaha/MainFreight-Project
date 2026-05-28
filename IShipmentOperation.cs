namespace MainfreightProject;

// This interface represents the Product role in the Factory Method pattern.
// It demonstrates abstraction because the staff workflow can work with one
// common operation type instead of depending directly on each concrete class.
//

// Transport, warehouse, customer service, and returned goods workflows all work
// with shipments, but each department has different responsibilities so having an interface gives the systme clear responsibility allocation
public interface IShipmentOperation
{
    void ExecuteOperation(Shipment shipment, Staff staffMember);
}