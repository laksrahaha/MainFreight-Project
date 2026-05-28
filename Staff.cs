using System;

namespace MainfreightProject;

//Staff extend the user class becasue it tkes infrmation of the staff from that class
//this will call the UpdateStatus method on the shipment object
//Shipment now notifies its registered status listeners after the status changes
//so tracking history/customer/staff updates are not manually controlled by Staff
public class Staff : User
{
    private string staffID;
    private string department;

    //intilazinig the objects of this class, using base to extend the class Generalzation
    public Staff(string userID, string name, string email, string staffID, string department)
        : base(userID, name, email)
    {
        this.staffID = staffID;
        this.department = department;
    }

    // this returns the message how the details fo the staff
    public string ViewStaffInfo()
    {
        return base.ViewProfile() + "Staff ID: " + staffID +
               "\nDepartment: " + department;
    }

    //this is the method for updating the status by the staff member
    public void UpdateShipmentStatus(Shipment shipment, string newStatus)
    {
        if (newStatus == "Delivered")
        {
            shipment.UpdateDeliveryStatus("Delivered");
        }
        else if (newStatus == "Returned")
        {
            shipment.UpdateDeliveryStatus("Returning");
        }
        else
        {
            shipment.UpdateDeliveryStatus("Not Delivered");
        }

        
        shipment.UpdateStatus(newStatus);

        Console.WriteLine("The Shipment status updated to: " + newStatus);
    }
}