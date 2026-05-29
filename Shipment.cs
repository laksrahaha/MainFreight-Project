using System;
using System.Collections.Generic;

namespace MainfreightProject;

public class Shipment
{
    private string shipmentID;
    private string status;
    private string currentLocation;
    private string deliveryStatus;

    //this list will contain all the tracking updates that are needed to be diplayed in trackingupdates
    private List<TrackingUpdate> trackingUpdate;

    //this list stores the status listeners that need to react when the shipment status changes
    //this supports the Observer pattern while still using Mainfreight-focused class names
    private List<IShipmentStatusListener> statusListeners;

    //this si the contructor class i ma intialising the objects , whihc are the shipment detials needed
    public Shipment(string shipmentID, string status, string currentLocation, string deliveryStatus)
    {
        this.shipmentID = shipmentID;
        this.status = status;
        this.currentLocation = currentLocation;
        this.deliveryStatus = deliveryStatus;

        // this is an empty list that is needed so updates cna be added alter
        trackingUpdate = new List<TrackingUpdate>();

        //this starts the listener list so tracking/customer/staff update handlers can be attached later
        statusListeners = new List<IShipmentStatusListener>();
    }

    public bool matchShipmentID(string inputID)
    {
        return shipmentID.Equals(inputID, StringComparison.OrdinalIgnoreCase);
    }

    public string getShipmentID()
    {
        return shipmentID;
    }

    public void TrackShipment()
    {
        //this is the message that will dislpay where the shipment currently is
        Console.WriteLine("Shipment " + shipmentID + " is located currently at " + currentLocation);
    }

    //te method adds the trackinupdate obect to the shipemt update list 
    public void addTrackingUpdate(TrackingUpdate updateShipment)
    {
        trackingUpdate.Add(updateShipment);
    }

    //this attaches a listener so it can react when the shipment status changes
    public void AttachStatusListener(IShipmentStatusListener listener)
    {
        statusListeners.Add(listener);
    }

    //this removes a listener if the system no longer needs that reaction
    public void RemoveStatusListener(IShipmentStatusListener listener)
    {
        statusListeners.Remove(listener);
    }

    //this notifies every attached listener when the shipment status changes
    //Shipment depends on the interface, not the concrete listener classes
    private void NotifyStatusListeners(string message)
    {
        foreach (IShipmentStatusListener listener in statusListeners)
        {
            listener.OnShipmentStatusChanged(this, message);
        }
    }

    //this mehtod tracks all the updates that are made or exxits in the shipment 
    //it does this by calling on trackinupdates list that we have created and the shipemnt lsit that sotres the update 
    public void viewTrackingHistory()
    {
        if (trackingUpdate.Count == 0)
        {
            Console.WriteLine("No tracking updates are available for this shipment.");
        }
        else
        {
            Console.WriteLine("Tracking updates for shipment " + shipmentID + ":");

            foreach (TrackingUpdate updateShipment in trackingUpdate)
            {
                Console.WriteLine(updateShipment.ViewUpdate());
                Console.WriteLine();
            }
        }
    }

    // this method is void because it needs to change the vlaue stord in the object , there is no need for it to send the value back
    public void UpdateStatus(string newStatus)
    {
        status = newStatus;

        if (newStatus == "Delivered")
        {
            deliveryStatus = "Delivered";
        }
        else if (newStatus == "Returned")
        {
            deliveryStatus = "Returned";
        }
        else
        {
            deliveryStatus = "Not Delivered";
        }

        //Observer pattern behaviour:
        //when shipment status changes, Shipment notifies every registered listener
        //so tracking history, customer visibility, and staff visibility react automatically
        NotifyStatusListeners("Shipment status updated to: " + newStatus);
    }

    public void UpdateCurrentLocation(string newLocation)
    {
        currentLocation = newLocation;
    }

    public void UpdateStatusAndLocation(string newStatus, string newLocation)
    {
        UpdateStatus(newStatus);
        UpdateCurrentLocation(newLocation);

        addTrackingUpdate(new TrackingUpdate(
            "UPD" + DateTime.Now.Ticks,
            DateTime.Now,
            "Shipment status updated to " + newStatus + " at " + newLocation + "."
        ));
    }

    public void UpdateDeliveryStatus(string newDeliveryStatus)
    {
        deliveryStatus = newDeliveryStatus;
    }

    public string getShipmentInfo()
    {
        // dislays the inofmration tha tis requested on one line
        return "Shipment ID:" + shipmentID +
        "\nShipment Status:" + status +
        "\nCurrent Location: " + currentLocation +
        "\nDelivery Status: " + deliveryStatus;
    }
}