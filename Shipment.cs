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
        private List<TrackingUpdates> trackingUpdates;

       //this si the contructor class i ma intialising the objects , whihc are the shipment detials needed
        public Shipment(string shipmentID,string status , string currentLocation, string deliveryStatus)
        {
            this.shipmentID = shipmentID;
            this.status = status;
            this.currentLocation = currentLocation;
            this.deliveryStatus = deliveryStatus;

        // this is an empty list that is needed so updates cna be added alter
            trackingUpdates = new List<TrackingUpdates>();
        }

        public void TrackShipment()
        {
            //this is the message that will dislpay where the shipment currently is
            Console.WriteLine("Shipment" + shipmentID + " is located currently at " + currentLocation);
        
        }

//te method adds the trackinupdate obect to the shipemt update list 
        public void AddTrackingUpdate(TrackingUpdates updateShipment)
        {

        trackingUpdates.Add(updateShipment);

        }

//this mehtod tracks all the updates that are made or exxits in the shipment 
//it does this by calling on trackinupdates list that we have created and the shipemnt lsit that sotres the update 
        public void ViewTrackingHistory()
        {
            if (trackingUpdates.Count == 0)
            {
                Console.WriteLine("No tracking updates are available for this shipment.");
            }
            else
            {
                Console.WriteLine("Tracking updates for shipment " + shipmentID + ":");

                foreach (TrackingUpdates updateShipment in trackingUpdates)
                {
                    Console.WriteLine(updateShipment.ViewUpdate());
                    Console.WriteLine();
                }
            }
        }


        // this method is void because it needs to change the vlaue stord in the object , there is no need for it to send the value back
        public void UpdateStatus(string newStatus)
        {
            status = newStatus;// not using this. becasue parameter names are not same
        
        }

        public string getShipmentInfo()
        {
            // di[lays the inofmration tha tis requested on one line]
            return "Shipment ID:" + shipmentID +
            "\nShipment Status:" + status +
            "\nCurrent Location: " + currentLocation +
            "\nDelivery Status: " + deliveryStatus;
        }
    
    }


