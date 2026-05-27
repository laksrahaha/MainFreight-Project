namespace MainfreightProject;

// AccessControlService checks whether a user is allowed to perform a workflow or not
// This separates the permission decisons from Program.cs and the shipment objects
//
// Mainfreight context:
// Customers should only view their assigned shipment while staff can update
// and manage shipment records if needed
// Keeping this responsibility separate supports and
// clearer business rules and better role-based control
public class AccessControlService
{
    public bool CanCustomerViewShipment(Customer customer, Shipment assignedShipment)
    {
        return customer != null && assignedShipment != null;
    }

    public bool CanStaffUpdateShipment(Staff staffMember, Shipment shipment)
    {
        return staffMember != null && shipment != null;
    }

    public bool CanStaffAddShipment(Staff staffMember)
    {
        return staffMember != null;
    }

    public bool CanStaffRunDepartmentOperation(Staff staffMember, Shipment shipment)
    {
        return staffMember != null && shipment != null;
    }
}