using System;

namespace MainfreightProject;

class Program
{
    static void Main(string[] args)
    {
        //these are the sample data i am using to create and test the menu from whihc i will be operating thhis main system 
        Customer customerMock = new Customer("User1", "Lakshmi", "lakshmi@email.com", "Custom1", "0211236567", "Auckland");
        Shipment shipmentMock = new Shipment("Ship1", "In Transit", "Auckland Depot", "Out for delivery");
        Staff staff1 = new Staff("User2", "Nainika", "staff@email.com", "Staff1", "Customer Service");

//the boolean varible keeps the proram going til the user chooses to break the program
        bool running = true;

        while (running)// the while loops keeps the menu running and loops it after each option till the user uses the boolean varible to exist the program
        {
            Console.WriteLine("\n--- Mainfreight Prototype ---");
            Console.WriteLine("1. View Customer Information");
            Console.WriteLine("2. Track Shipment");
            Console.WriteLine("3. View Shipment Information");
            Console.WriteLine("4. Update Shipment Status");
            Console.WriteLine("5. View Staff Information");
            Console.WriteLine("6. Exit program");
            Console.Write("Enter your choice: ");

//this records the user's choice an displays it accordingly 
            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                //displays the cusotmer detials 
                    Console.WriteLine(customerMock.ViewcustomerInfo());
                    break;

// diaplsy where the shipment is calls on trackshipment form the shipment class
                case "2":
                    customerMock.TrackShipment("Staff1");
                    shipmentMock.TrackShipment();
                    break;

//displays the shipment information
                case "3":
                    Console.WriteLine(shipmentMock.getShipmentInfo());
                    break;

//allows the user to enter a new shipment status and save it, it updates it
                case "4":
                    Console.Write("Enter the new shipment status please: ");
                    string newStatus = Console.ReadLine();
                    staff1.UpdateShipmentStatus(shipmentMock, newStatus);
                    break;

//this allows uer to view staff infomration
                case "5":
                    Console.WriteLine(staff1.ViewStaffInfo());
                    break;

//allows for the user to break the program then thanks them for using it 
                case "6":
                    running = false;
                    Console.WriteLine("Thank You for choosing us");
                    break;

                    // this is the error handling
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
