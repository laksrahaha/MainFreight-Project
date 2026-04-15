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
            Console.WriteLine("\n***Mainfreight Prototype ***");
            Console.WriteLine("1. Customer Menu");
            Console.WriteLine("2. Staff Menu");
            Console.WriteLine("3. Exit program");
            Console.Write("Enter your choice: ");

            //this records the user's choice an displays it accordingly 
            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    ShowCustomerMenu(customerMock, shipmentMock);
                    break;

                case "2":
                    ShowStaffMenu(staff1, shipmentMock);
                    break;

                //allows for the user to break the program then thanks them for using it 
                case "3":
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

    //this method groups all the customer options together in one nested menu
    static void ShowCustomerMenu(Customer customerMock, Shipment shipmentMock)
    {
        bool customerMenu = true;

        while (customerMenu)
        {
            Console.WriteLine("\n--- Customer Menu ---");
            Console.WriteLine("1. View Customer Information");
            Console.WriteLine("2. Track Shipment");
            Console.WriteLine("3. View Shipment Information");
            Console.WriteLine("4. View Tracking History");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("Enter your choice: ");

            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    //displays the cusotmer detials 
                    Console.WriteLine(customerMock.ViewcustomerInfo());
                    Pause();
                    break;

                // diaplsy where the shipment is calls on trackshipment form the shipment class
                case "2":
                    shipmentMock.TrackShipment();
                    Pause();
                    break;

                //displays the shipment information
                case "3":
                    Console.WriteLine(shipmentMock.getShipmentInfo());
                    Pause();
                    break;

                case "4":
                    shipmentMock.ViewTrackingHistory();
                    Pause();
                    break;

                case "5":
                    customerMenu = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    //this method groups all the staff options together in one menu whihc is nested
    static void ShowStaffMenu(Staff staff1, Shipment shipmentMock)
    {
        bool staffMenu = true;

        while (staffMenu)
        {
            Console.WriteLine("\n--- Staff Menu ---");
            Console.WriteLine("1. View Staff Information");
            Console.WriteLine("2. Update Shipment Status");
            Console.WriteLine("3. View Tracking History");
            Console.WriteLine("4. Back to Main Menu");
            Console.Write("Enter your choice: ");

            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    Console.WriteLine(staff1.ViewStaffInfo());
                    Pause();
                    break;

                case "2":
                    Console.Write("Enter the new shipment status please: ");
                    string newStatus = Console.ReadLine();
                    staff1.UpdateShipmentStatus(shipmentMock, newStatus);
                    Pause();
                    break;

                case "3":
                    shipmentMock.ViewTrackingHistory();
                    Pause();
                    break;

                case "4":
                    staffMenu = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    //this just pauses the program so the user can read the output before the menu comes again
    static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}