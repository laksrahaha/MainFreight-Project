using System;
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
        return UpdateShipmentStatus(staffMember, shipmentID, newStatus, "");
    }

    public string UpdateShipmentStatus(Staff staffMember, string shipmentID, string newStatus, string newLocation)
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

        if (string.IsNullOrWhiteSpace(newLocation))
        {
            staffMember.UpdateShipmentStatus(shipment, newStatus);
        }
        else
        {
            shipment.UpdateStatusAndLocation(newStatus, newLocation);
        }

        shipmentRepo.SaveChanges();

        if (string.IsNullOrWhiteSpace(newLocation))
        {
            return "Shipment " + shipmentID + " has been updated to " + newStatus + ".";
        }

        return "Shipment " + shipmentID + " has been updated to " + newStatus + " at " + newLocation + ".";
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

        bool added = shipmentRepo.AddShipment(newShipment);

        if (added)
        {
            return "Shipment " + shipmentID + " has been added successfully.";
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
            return "Access denied. Staff member is not allowed to record department processing for this shipment.";
        }

        try
        {
            IShipmentOperation selectedOperation = shipmentOperationFactory.CreateOperation(operationType);

            selectedOperation.ExecuteOperation(shipment, staffMember);

            string processingLabel = GetProcessingLabel(operationType);
            string trackingMessage = processingLabel + " has been recorded for shipment " + shipmentID + ".";

            shipment.addTrackingUpdate(new TrackingUpdate(
                "UPD" + DateTime.Now.Ticks,
                DateTime.Now,
                trackingMessage
            ));

            shipmentRepo.SaveChanges();

            return trackingMessage;
        }
        catch
        {
            return "Invalid department processing type selected.";
        }
    }

    private string GetProcessingLabel(string operationType)
    {
        string selectedType = operationType.Trim().ToLower();

        if (selectedType.Contains("customer"))
        {
            return "Customer Service Processing";
        }

        if (selectedType.Contains("warehouse"))
        {
            return "Warehouse Processing";
        }

        if (selectedType.Contains("return"))
        {
            return "Returned Goods Processing";
        }

        return "Transport Processing";
    }
}