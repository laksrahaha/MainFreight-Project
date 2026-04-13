using System;
using System.Runtime.CompilerServices;

namespace MainfreightProject;

//the class contains its user detials form the user class and reprresntd the customer
public class Customer : User
{ 
    private string customerID;
    private string contactDetails;
    private string address;

// the contructor inherits user detials and uses it own customer detail objects aswell
    public Customer(string userID, string name, string email,string customerID, string contactDetails, string address) : base (userID, name, email)
    {
        this.customerID = customerID;
        this.contactDetails = contactDetails;
        this.address = address;
    }

// this lets the user know which shipment they are takcing by calling on shipmentID
    public void TrackShipment(string shipmentID)
    {
        Console.WriteLine("Customer is tracking the shipment: " + shipmentID);

    }

    public string ViewcustomerInfo()
    {
        //this returns the customer detials in one line
        return "Customer ID: " + customerID +
                   "\nContact Details: " + contactDetails +
                   "\nAddress: " + address;
    }

}

    


