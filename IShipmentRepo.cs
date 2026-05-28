using System.Collections.Generic;

namespace MainfreightProject;

// IShipmentRepo defines the storage/search contract for shipment data.
// This supports abstraction because the controller can work with a repository so it hides the internal work
// interface instead of depending directly on a list or text file.
//

// Shipment data needs to be searched, listed, added, and saved, Keeping this
// behind a repository interface reduces the amount of storage logic inside once again providing us with abstraction
public interface IShipmentRepo
{
    List<Shipment> GetAllShipments();

    Shipment FindShipmentByID(string shipmentID);

    bool ShipmentExists(string shipmentID);

    bool AddShipment(Shipment shipment);

    void SaveChanges();
}