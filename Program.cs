using System;
using System.Collections.Generic;

namespace MainfreightProject;

class Program
{
    static void Main(string[] args)
    {
        //these are the sample data i am using to create and test the menu from whihc i will be operating thhis main system 
        Customer customerMock = new Customer("User1", "Lakshmi", "lakshmi@email.com", "Custom1", "0211236567", "Auckland");

        Staff staff1 = new Staff("User2", "Nainika", "staff@email.com", "Staff1", "Customer Service");

        //using a list to stroe all the sample data and also having a alist means the staff user can append theri information to this list 
        List<Shipment> shipments = new List<Shipment>
        {
            new Shipment("Ship1", "In Transit", "Auckland Depot", "Out for delivery"),
            new Shipment("Ship2", "Delivered", "Manukau Hub", "Delivered"),
            new Shipment("Ship3", "Delayed", "Hamilton Depot", "Delayed")
        };

        //this assigns one shipment to the customer side of the demo
        Shipment customerShipment = shipments[0];

        //these starter tracking updates make the system feel more real when demoing
        shipments[0].AddTrackingUpdate(
            new TrackingUpdate(
                "UPD001",
                DateTime.Now,
                "Shipment arrived at Auckland Depot."
            )
        );

        shipments[1].AddTrackingUpdate(
            new TrackingUpdate(
                "UPD002",
                DateTime.Now,
                "Shipment delivered successfully."
            )
        );

        shipments[2].AddTrackingUpdate(
            new TrackingUpdate(
                "UPD003",
                DateTime.Now,
                "Shipment delayed due to transport issue."
            )
        );

        //the boolean varible keeps the proram going til the user chooses to break the program
        bool running = true;

        while (running)// the while loops keeps the menu running and loops it after each option till the user uses the boolean varible to exist the program
        {
            Console.WriteLine("\n*** Mainfreight Prototype ***");
            Console.WriteLine("1. Customer Menu");
            Console.WriteLine("2. Staff Menu");
            Console.WriteLine("3. Exit program");
            Console.Write("Enter your choice: ");

            //this records the user's choice an displays it accordingly 
            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    ShowCustomerMenu(customerMock, customerShipment);
                    break;

                case "2":
                    ShowStaffMenu(staff1, shipments);
                    break;

                //allows for the user to break the program then thanks them for using it 
                case "3":
                    if (ConfirmAction("Are you sure you want to exit the program? (yes/no): "))
                    {
                        running = false;
                        Console.WriteLine("Thank You for choosing us");
                    }
                    break;

                // this is the error handling
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    //this method groups all the customer options together in one nested menu
    static void ShowCustomerMenu(Customer customerMock, Shipment customerShipment)
    {
        bool customerMenu = true;

        while (customerMenu)
        {
            Console.WriteLine("\n--- Customer Menu ---");
            Console.WriteLine("1. View Customer Information");
            Console.WriteLine("2. Update Contact Details");
            Console.WriteLine("3. Update Address");
            Console.WriteLine("4. Track My Shipment");
            Console.WriteLine("5. View My Shipment Information");
            Console.WriteLine("6. View My Tracking History");
            Console.WriteLine("7. Back to Main Menu");
            Console.Write("Enter your choice: ");

            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    //displays the cusotmer detials 
                    Console.WriteLine(customerMock.ViewcustomerInfo());
                    Pause();
                    break;

                case "2":
                    //this lets the customer edit their contact details during runtime
                    string newContact = ReadNonEmptyInput("Enter the new contact details: ");

                    if (!ConfirmAction("Are you sure you want to update the contact details? (yes/no): "))
                    {
                        Console.WriteLine("Contact details update cancelled.");
                        Pause();
                        break;
                    }

                    customerMock.updateContactDetails(newContact);
                    Console.WriteLine("Customer contact details updated successfully.");
                    Pause();
                    break;

                case "3":
                    //this lets the customer edit their address during runtime
                    string newAddress = ReadNonEmptyInput("Enter the new address: ");

                    if (!ConfirmAction("Are you sure you want to update the address? (yes/no): "))
                    {
                        Console.WriteLine("Address update cancelled.");
                        Pause();
                        break;
                    }

                    customerMock.UpdateAddress(newAddress);
                    Console.WriteLine("Customer address updated successfully.");
                    Pause();
                    break;

                case "4":
                    //customer can only track their own assigned shipment
                    customerShipment.TrackShipment();

                    if (ConfirmAction("Would you like to view the tracking history for this shipment as well? (yes/no): "))
                    {
                        customerShipment.viewTrackingHistory();
                    }

                    Pause();
                    break;

                case "5":
                    //customer can only view their own shipment info
                    Console.WriteLine(customerShipment.getShipmentInfo());
                    Pause();
                    break;

                case "6":
                    //customer can only view their own shipment history
                    customerShipment.viewTrackingHistory();
                    Pause();
                    break;

                case "7":
                    //this confirms before leaving the customer menu
                    if (ConfirmAction("Are you sure you want to go back to the main menu? (yes/no): "))
                    {
                        customerMenu = false;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    //this method groups all the staff options together in one menu whihc is nested
    static void ShowStaffMenu(Staff staff1, List<Shipment> shipments)
    {
        bool staffMenu = true;

        while (staffMenu)
        {
            Console.WriteLine("\n--- Staff Menu ---");
            Console.WriteLine("1. View Staff Information");
            Console.WriteLine("2. Update Shipment Status");
            Console.WriteLine("3. View Tracking History");
            Console.WriteLine("4. View All Shipments");
            Console.WriteLine("5. Add New Shipment");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("Enter your choice: ");

            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    Console.WriteLine(staff1.ViewStaffInfo());
                    Pause();
                    break;

                case "2":
                    //staff can select which shipment they want to update instead of the system assuming one shipment
                    Shipment updateShipment = PromptForShipment(shipments, "Enter shipment ID to update (or type back): ");

                    if (updateShipment == null)
                    {
                        Pause();
                        break;
                    }

                    //using preset status choices makes the system feel more controlled and reduces invalid input
                    string[] statusOptions = { "In Transit", "Out for delivery", "Delivered", "Delayed", "Returned" };

                    Console.WriteLine("\nChoose the new shipment status:");
                    for (int i = 0; i < statusOptions.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + statusOptions[i]);
                    }

                    string statusChoice = ReadNonEmptyInput("Enter your choice: ");
                    int statusIndex;

                    if (!int.TryParse(statusChoice, out statusIndex) || statusIndex < 1 || statusIndex > statusOptions.Length)
                    {
                        Console.WriteLine("Invalid status option selected.");
                        Pause();
                        break;
                    }

                    string newStatus = statusOptions[statusIndex - 1];

                    if (!ConfirmAction("Are you sure you want to update the shipment status? (yes/no): "))
                    {
                        Console.WriteLine("Shipment status update cancelled.");
                        Pause();
                        break;
                    }

                    //this shows object interaction because staff updates the chosen shipment object
                    staff1.UpdateShipmentStatus(updateShipment, newStatus);
                    Console.WriteLine(updateShipment.getShipmentInfo());
                    Pause();
                    break;

                case "3":
                    //staff can inspect the history of any shipment in the runtime list
                    Shipment historyShipment = PromptForShipment(shipments, "Enter shipment ID to view tracking history (or type back): ");

                    if (historyShipment != null)
                    {
                        historyShipment.viewTrackingHistory();
                    }

                    Pause();
                    break;

                case "4":
                    //this gives a quick overview of all current shipments in the system
                    Console.WriteLine("\n--- All Shipments ---");

                    foreach (Shipment shipment in shipments)
                    {
                        Console.WriteLine(shipment.getShipmentInfo());
                        Console.WriteLine();
                    }

                    Pause();
                    break;

                case "5":
                    //this lets staff append a brand new shipment into the list during runtime
                    string newShipmentID = ReadNonEmptyInput("Enter new shipment ID: ");

                    Shipment existingShipment = FindShipmentByID(shipments, newShipmentID);

                    if (existingShipment != null)
                    {
                        Console.WriteLine("A shipment with that ID already exists. Please use a different shipment ID.");
                        Pause();
                        break;
                    }

                    string newLocation = ReadNonEmptyInput("Enter current location: ");

                    string[] shipmentStatusOptions = { "In Transit", "Out for delivery", "Delivered", "Delayed", "Returned" };

                    Console.WriteLine("\nChoose the shipment status:");
                    for (int i = 0; i < shipmentStatusOptions.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + shipmentStatusOptions[i]);
                    }

                    string shipmentStatusChoice = ReadNonEmptyInput("Enter your choice: ");
                    int shipmentStatusIndex;

                    if (!int.TryParse(shipmentStatusChoice, out shipmentStatusIndex) || shipmentStatusIndex < 1 || shipmentStatusIndex > shipmentStatusOptions.Length)
                    {
                        Console.WriteLine("Invalid shipment status option selected.");
                        Pause();
                        break;
                    }

                    string newShipmentStatus = shipmentStatusOptions[shipmentStatusIndex - 1];

                    Console.WriteLine("\nChoose the delivery status:");
                    for (int i = 0; i < shipmentStatusOptions.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + shipmentStatusOptions[i]);
                    }

                    string deliveryStatusChoice = ReadNonEmptyInput("Enter your choice: ");
                    int deliveryStatusIndex;

                    if (!int.TryParse(deliveryStatusChoice, out deliveryStatusIndex) || deliveryStatusIndex < 1 || deliveryStatusIndex > shipmentStatusOptions.Length)
                    {
                        Console.WriteLine("Invalid delivery status option selected.");
                        Pause();
                        break;
                    }

                    string newDeliveryStatus = shipmentStatusOptions[deliveryStatusIndex - 1];

                    if (!ConfirmAction("Are you sure you want to add this new shipment? (yes/no): "))
                    {
                        Console.WriteLine("New shipment creation cancelled.");
                        Pause();
                        break;
                    }

                    Shipment newShipment = new Shipment(newShipmentID, newShipmentStatus, newLocation, newDeliveryStatus);
                    shipments.Add(newShipment);

                    Console.WriteLine("New shipment added successfully.");
                    Pause();
                    break;

                case "6":
                    //this confirms before leaving the staff menu
                    if (ConfirmAction("Are you sure you want to go back to the main menu? (yes/no): "))
                    {
                        staffMenu = false;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    //this helper keeps asking until the user enters something meningful
    static string ReadNonEmptyInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.WriteLine("Input cannot be left blank. Please try again.");
        }
    }

    //this helper looks through the shipment list and returns the matching shipment object
    static Shipment FindShipmentByID(List<Shipment> shipments, string enteredID)
    {
        foreach (Shipment shipment in shipments)
        {
            if (shipment.matchShipmentID(enteredID))
            {
                return shipment;
            }
        }

        return null;
    }

    //this helper keeps asking for a shipment id until a valid one is found or the user types back
    static Shipment PromptForShipment(List<Shipment> shipments, string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string enteredID = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(enteredID))
            {
                Console.WriteLine("Shipment ID cannot be left blank. Please try again or type back to return.");
                continue;
            }

            if (enteredID.Equals("back", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            Shipment foundShipment = FindShipmentByID(shipments, enteredID);

            if (foundShipment != null)
            {
                return foundShipment;
            }

            Console.WriteLine("Shipment not found. Please try again or type back to return.");
        }
    }

    //this helper is used before important actions like leaving or updating
    static bool ConfirmAction(string message)
    {
        while (true)
        {
            Console.Write(message);
            string choice = Console.ReadLine();

            if (choice.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                choice.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (choice.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                choice.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Console.WriteLine("Please enter yes or no.");
        }
    }

    //this just pauses the program so the user can read the output before the menu comes again
    static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}