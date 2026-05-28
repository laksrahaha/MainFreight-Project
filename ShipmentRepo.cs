using System.Collections.Generic;
using System.IO;

namespace MainfreightProject;

// ShipmentRepo manages shipment storage and lookup.
// It represents the repository layer in the refined UML design.
//

// Shipment searching and saving should not be spreadout throughout menu code.
// This class centralises shipment data handling, making the system easier to
// maintain and extend laterin the future.
public class ShipmentRepo : IShipmentRepo
{
    private List<Shipment> shipments;
    private string shipmentFilePath;

    public ShipmentRepo(List<Shipment> shipments, string shipmentFilePath)
    {
        this.shipments = shipments;
        this.shipmentFilePath = shipmentFilePath;
    }

    public List<Shipment> GetAllShipments()
    {
        return shipments;
    }

    public Shipment FindShipmentByID(string shipmentID)
    {
        foreach (Shipment shipment in shipments)
        {
            if (shipment.matchShipmentID(shipmentID))
            {
                return shipment;
            }
        }

        return null;
    }

    public bool ShipmentExists(string shipmentID)
    {
        return FindShipmentByID(shipmentID) != null;
    }

    public bool AddShipment(Shipment shipment)
    {
        if (shipment == null)
        {
            return false;
        }

        if (ShipmentExists(shipment.getShipmentID()))
        {
            return false;
        }

        shipments.Add(shipment);
        SaveChanges();
        return true;
    }

    public void SaveChanges()
    {
        List<string> lines = new List<string>();

        foreach (Shipment shipment in shipments)
        {
            string shipmentInfo = shipment.getShipmentInfo();
            string[] shipmentParts = ExtractShipmentData(shipmentInfo);

            lines.Add(shipmentParts[0] + "|" + shipmentParts[1] + "|" + shipmentParts[2] + "|" + shipmentParts[3]);
        }

        File.WriteAllLines(shipmentFilePath, lines);
    }

    private string[] ExtractShipmentData(string shipmentInfo)
    {
        string[] lines = shipmentInfo.Split('\n');

        string shipmentID = lines[0].Replace("Shipment ID:", "").Trim();
        string shipmentStatus = lines[1].Replace("Shipment Status:", "").Trim();
        string currentLocation = lines[2].Replace("Current Location:", "").Trim();
        string deliveryStatus = lines[3].Replace("Delivery Status:", "").Trim();

        return new string[] { shipmentID, shipmentStatus, currentLocation, deliveryStatus };
    }
}