using System;

namespace MainfreightProject;

//Staff extend the user class becasue it tkes infrmation of the staff from that class
public class Staff:User

{
    private string staffID;
    private string department;

//intilazinig the objects of this class, using base to extend the class., Generalzation
    public Staff(string userID, string name, string email, string staffID, string department)
        : base(userID, name, email)
    {
        this.staffID = staffID;
        this.department = department;
    }

// this returns the message how the details fo the staff
    public string ViewStaffInfo()
    {
        return "Staff ID: " + staffID +
               "\nDepartment: " + department;
    }

//this is the method for updating the status by the staff member
    public void UpdateShipmentStatus(Shipment shipment, string newStatus)
    {
        //this will the updatestauts method on the shipment object then give it the new value that they wih to  update
        shipment.UpdateStatus(newStatus);
        Console.WriteLine("Shipment status updated to:" +  newStatus);
    }
}

