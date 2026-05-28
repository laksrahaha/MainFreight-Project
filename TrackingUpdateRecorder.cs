using System;

namespace MainfreightProject;

// TrackingUpdateRecorder records a tracking update when a shipment status changes.
//

// Tracking history should automatically reflect important shipment changes.
// This class reuses the existing TrackingUpdate class and Shipment.addTrackingUpdate() method instead of creating a separate tracking system.
public class TrackingUpdateRecorder : IShipmentStatusListener
{
    public void OnShipmentStatusChanged(Shipment shipment, string message)
    {
        TrackingUpdate newUpdate = new TrackingUpdate(
            "UPD" + DateTime.Now.Ticks,
            DateTime.Now,
            message
        );

        shipment.addTrackingUpdate(newUpdate);

        Console.WriteLine("Tracking history recorded for shipment " + shipment.getShipmentID() + ".");
    }
}