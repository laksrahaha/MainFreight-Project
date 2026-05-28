namespace MainfreightProject;

// This interface defines a common contract for classes that need to react
// when a shipment status changes.
//

// This acts as the Observer interface. Shipment can notify listeners through
// this interface without depending on the concrete listener classes.

// A shipment status change can affect tracking history, customer visibility,
// and staff visibility. This interface keeps those reactions separate from
// Staff and Program.cs.
public interface IShipmentStatusListener
{
    void OnShipmentStatusChanged(Shipment shipment, string message);
}