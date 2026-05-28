using System.Collections.Generic;

namespace MainfreightProject;

// ShipmentController coordinates shipment workflows.
// It sits between the menu layer and the repo/domain and service classes

// this helps becasue Program.cs should mainly collect input and display output. The controller
// coordinates business workflows such as updating shipment status, adding a
// shipment, and running a department shipment operation
//
public class ShipmentController
{
    private IShipmentRepo shipmentRepo;
    private AccessControlService accessControlService;
    private ShipmentOperationFactory shipmentOperationFactory;

    public ShipmentController(
        IShipmentRepo shipmentRepo,
        AccessControlService accessControlService,
        ShipmentOperationFactory shipmentOperationFactory)
    {
        this.shipmentRepo = shipmentRepo;
        this.accessControlService = accessControlService;
        this.shipmentOperationFactory = shipmentOperationFactory;
    }

    public List<Shipment> GetAllShipments()
    {
        return shipmentRepo.GetAllShipments();
    }

    public Shipment FindShipmentByID(string shipmentID)
    {
        return shipmentRepo.FindShipmentByID(shipmentID);
    }

    public string UpdateShipmentStatus(Staff staffMember, string shipmentID, string newStatus)
    {
        Shipment shipment = shipmentRepo.FindShipmentByID(shipmentID);

        if (shipment == null)
        {
            return "Shipment not found.";
        }

        if (!accessControlService.CanStaffUpdateShipment(staffMember, shipment))
        {
            return "Access denied. Staff member is not allowed to update this shipment.";
        }

        staffMember.UpdateShipmentStatus(shipment, newStatus);
        shipmentRepo.SaveChanges();

        return "Shipment status workflow completed through ShipmentController.";
    }

    public string AddNewShipment(
        Staff staffMember,
        string shipmentID,
        string shipmentStatus,
        string currentLocation,
        string deliveryStatus)
    {
        if (!accessControlService.CanStaffAddShipment(staffMember))
        {
            return "Access denied. Staff member is not allowed to add shipments.";
        }

        if (shipmentRepo.ShipmentExists(shipmentID))
        {
            return "A shipment with that ID already exists. Please use a different shipment ID.";
        }

        Shipment newShipment = new Shipment(shipmentID, shipmentStatus, currentLocation, deliveryStatus);

        //new shipments also need the same status listeners as the startup shipments
        //so future status changes can update tracking history and visibility outputs
        RegisterStatusListenersForShipment(newShipment);

        bool added = shipmentRepo.AddShipment(newShipment);

        if (added)
        {
            return "New shipment added successfully through ShipmentController.";
        }

        return "New shipment could not be added.";
    }

    public string RunDepartmentShipmentOperation(Staff staffMember, string shipmentID, string operationType)
    {
        Shipment shipment = shipmentRepo.FindShipmentByID(shipmentID);

        if (shipment == null)
        {
            return "Shipment not found.";
        }

        if (!accessControlService.CanStaffRunDepartmentOperation(staffMember, shipment))
        {
            return "Access denied. Staff member is not allowed to run this department operation.";
        }

        try
        {
            IShipmentOperation selectedOperation = shipmentOperationFactory.CreateOperation(operationType);

            selectedOperation.ExecuteOperation(shipment, staffMember);

            return "Department shipment operation completed through ShipmentController.";
        }
        catch
        {
            return "Invalid department shipment operation selected.";
        }
    }

    private void RegisterStatusListenersForShipment(Shipment shipment)
    {
        shipment.AttachStatusListener(new TrackingUpdateRecorder());
        shipment.AttachStatusListener(new CustomerStatusNotifier());
        shipment.AttachStatusListener(new StaffStatusNotifier());
    }
}