using System;
using System.Collections.Generic;
using System.IO;

namespace MainfreightProject;

class Program
{
    static void Main(string[] args)
    {
        //these are the sample data i am using to create and test the menu from whihc i will be operating thhis main system uing lsits to store this information
        List<Customer> customers = new List<Customer>
        {
            new Customer("User1", "Lakshmi", "lakshmi@email.com", "Custom1", "0211236567", "Auckland"),
            new Customer("User3", "Asha", "asha@email.com", "Custom2", "0224567890", "Manukau")
        };

        List<Staff> staffMembers = new List<Staff>
        {
            new Staff("User2", "Nainika", "staff@email.com", "Staff1", "Customer Service"),
            new Staff("User4", "Riya", "riya@email.com", "Staff2", "Operations")
        };

        //this file path is used so shipment data can be saved and loaded again next time the program runs
        string shipmentFilePath = "shipments.txt";

        //using a list to stroe all the sample data and also having a alist means the staff user can append theri information to this list 
        List<Shipment> shipments = LoadShipmentsFromFile(shipmentFilePath);

        //this is just a fallback so if there is no file yet the program still has starting data to work with
        if (shipments.Count == 0)
        {
            shipments = new List<Shipment>
            {
                new Shipment("Ship1", "In Transit", "Auckland Depot", "Not Delivered"),
                new Shipment("Ship2", "Delivered", "Manukau Hub", "Delivered"),
                new Shipment("Ship3", "Delayed", "Hamilton Depot", "Not Delivered")
            };

            SaveShipmentsToFile(shipments, shipmentFilePath);
        }

        //this attaches status listeners to every shipment so status updates can trigger
        //tracking history, customer visibility, and staff visibility reactions
        foreach (Shipment shipment in shipments)
        {
            RegisterShipmentStatusListeners(shipment);
        }

        //this assigns one shipment to the customer side of the demo
        Dictionary<Customer, Shipment> customerShipments = new Dictionary<Customer, Shipment>();

        //this just links the demo customers to demo shipments so when a customer is chosen the menu knows which shipment belongs to them
        if (customers.Count > 0 && shipments.Count > 0)
        {
            customerShipments.Add(customers[0], shipments[0]);
        }

        if (customers.Count > 1 && shipments.Count > 1)
        {
            customerShipments.Add(customers[1], shipments[1]);
        }

        //these starter tracking updates make the system feel more real when demoing
        if (shipments.Count > 0)
        {
            shipments[0].addTrackingUpdate(
                new TrackingUpdate(
                    "UPD001",
                    DateTime.Now,
                    "Shipment arrived at Auckland Depot."
                )
            );
        }

        if (shipments.Count > 1)
        {
            shipments[1].addTrackingUpdate(
                new TrackingUpdate(
                    "UPD002",
                    DateTime.Now,
                    "Shipment delivered successfully."
                )
            );
        }

        if (shipments.Count > 2)
        {
            shipments[2].addTrackingUpdate(
                new TrackingUpdate(
                    "UPD003",
                    DateTime.Now,
                    "Shipment delayed due to transport issue."
                )
            );
        }

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
                    //this lets the system ask which customer is using the menu instead of just assuming the same one every time
                    Customer selectedCustomer = SelectCustomer(customers);

                    if (selectedCustomer != null)
                    {
                        if (customerShipments.ContainsKey(selectedCustomer))
                        {
                            ShowCustomerMenu(selectedCustomer, customerShipments[selectedCustomer]);
                        }
                        else
                        {
                            Console.WriteLine("No shipment is currently assigned to this customer.");
                            Pause();
                        }
                    }
                    break;

                case "2":
                    //same idea here for staff so the system shows which staff member is entering instead of forcing one default person
                    Staff selectedStaff = SelectStaff(staffMembers);

                    if (selectedStaff != null)
                    {
                        ShowStaffMenu(selectedStaff, shipments, shipmentFilePath);
                    }
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
    static void ShowStaffMenu(Staff staff1, List<Shipment> shipments, string shipmentFilePath)
    {
        bool staffMenu = true;

        //this connects the staff menu to the refined UML structure
        //the menu still handles user input, but the controller coordinates the shipment workflows
        IShipmentRepo shipmentRepo = new ShipmentRepo(shipments, shipmentFilePath);
        AccessControlService accessControlService = new AccessControlService();
        ShipmentOperationFactory shipmentOperationFactory = new ShipmentOperationFactory();

        ShipmentController shipmentController = new ShipmentController(
            shipmentRepo,
            accessControlService,
            shipmentOperationFactory
        );

        while (staffMenu)
        {
            Console.WriteLine("\n--- Staff Menu ---");
            Console.WriteLine("1. View Staff Information");
            Console.WriteLine("2. Update Shipment Status");
            Console.WriteLine("3. View Tracking History");
            Console.WriteLine("4. View All Shipments");
            Console.WriteLine("5. Add New Shipment");
            Console.WriteLine("6. Run Department Shipment Operation");
            Console.WriteLine("7. Back to Main Menu");

            string Userchoice = Console.ReadLine();

            switch (Userchoice)
            {
                case "1":
                    Console.WriteLine(staff1.ViewStaffInfo());
                    Pause();
                    break;

                case "2":
                    //displays the shipments at the start itself so that the staff can see the id in the start itself, when they decide they want to choose view a certian cshients detials
                    Console.WriteLine("\n--- All Shipments ---");

                    foreach (Shipment shipment in shipmentController.GetAllShipments())
                    {
                        Console.WriteLine(shipment.getShipmentInfo());
                        Console.WriteLine();
                    }

                    //staff can select which shipment they want to update instead of the system assuming one shipment
                    Shipment updateShipment = PromptForShipment(shipmentController.GetAllShipments(), "Enter shipment ID to update (or type back): ");

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

                    //this shows object interaction because staff requests the update,
                    //but the ShipmentController now coordinates the workflow through the refined UML structure
                    string updateResult = shipmentController.UpdateShipmentStatus(
                        staff1,
                        updateShipment.getShipmentID(),
                        newStatus
                    );

                    Console.WriteLine(updateResult);
                    Console.WriteLine(updateShipment.getShipmentInfo());
                    Pause();
                    break;

                case "3":
                    //staff can inspect the history of any shipment in the runtime list
                    Shipment historyShipment = PromptForShipment(shipmentController.GetAllShipments(), "Enter shipment ID to view tracking history (or type back): ");

                    if (historyShipment != null)
                    {
                        historyShipment.viewTrackingHistory();
                    }

                    Pause();
                    break;

                case "4":
                    //this gives a quick overview of all current shipments in the system
                    Console.WriteLine("\n--- All Shipments ---");

                    foreach (Shipment shipment in shipmentController.GetAllShipments())
                    {
                        Console.WriteLine(shipment.getShipmentInfo());
                        Console.WriteLine();
                    }

                    Pause();
                    break;

                case "5":
                    //this lets staff append a brand new shipment into the list during runtime
                    string newShipmentID = ReadNonEmptyInput("Enter new shipment ID: ");

                    Shipment existingShipment = shipmentController.FindShipmentByID(newShipmentID);

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

                    string[] deliveryStatusOptions = { "Delivered", "Not Delivered", "Returning" };

                    Console.WriteLine("\nChoose the delivery status:");
                    for (int i = 0; i < deliveryStatusOptions.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + deliveryStatusOptions[i]);
                    }

                    string deliveryStatusChoice = ReadNonEmptyInput("Enter your choice: ");
                    int deliveryStatusIndex;

                    if (!int.TryParse(deliveryStatusChoice, out deliveryStatusIndex) || deliveryStatusIndex < 1 || deliveryStatusIndex > deliveryStatusOptions.Length)
                    {
                        Console.WriteLine("Invalid delivery status option selected.");
                        Pause();
                        break;
                    }

                    string newDeliveryStatus = deliveryStatusOptions[deliveryStatusIndex - 1];

                    if (!ConfirmAction("Are you sure you want to add this new shipment? (yes/no): "))
                    {
                        Console.WriteLine("New shipment creation cancelled.");
                        Pause();
                        break;
                    }

                    //this sends the new shipment details to the controller so the refined UML structure
                    //handles duplicate checking, shipment creation, repository storage, and saving
                    string addResult = shipmentController.AddNewShipment(
                        staff1,
                        newShipmentID,
                        newShipmentStatus,
                        newLocation,
                        newDeliveryStatus
                    );

                    Console.WriteLine(addResult);
                    Pause();
                    break;

                case "6":
                    // Factory Method demo:
                    // Staff choose a department operation, but Program.cs does not directly create
                    // TransportShipmentOperation, WarehouseShipmentOperation, CustomerServiceOperation,
                    // or ReturnedGoodsOperation. It asks ShipmentOperationFactory to create the
                    // correct object through the IShipmentOperation interface.
                    Shipment operationShipment = PromptForShipment(shipmentController.GetAllShipments(), "Enter shipment ID for department operation (or type back): ");

                    if (operationShipment == null)
                    {
                        Pause();
                        break;
                    }

                    Console.WriteLine("\nChoose department operation type:");
                    Console.WriteLine("1. Transport Operation");
                    Console.WriteLine("2. Warehouse Operation");
                    Console.WriteLine("3. Customer Service Operation");
                    Console.WriteLine("4. Returned Goods Operation");

                    string operationChoice = ReadNonEmptyInput("Enter your choice: ");
                    string operationType = "";

                    switch (operationChoice)
                    {
                        case "1":
                            operationType = "transport";
                            break;

                        case "2":
                            operationType = "warehouse";
                            break;

                        case "3":
                            operationType = "customerservice";
                            break;

                        case "4":
                            operationType = "return";
                            break;

                        default:
                            Console.WriteLine("Invalid operation type selected.");
                            Pause();
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(operationType))
                    {
                        break;
                    }

                    //this keeps the Factory Method workflow, but routes it through ShipmentController
                    //so the pattern is now part of the refined Mainfreight system architecture
                    string operationResult = shipmentController.RunDepartmentShipmentOperation(
                        staff1,
                        operationShipment.getShipmentID(),
                        operationType
                    );

                    Console.WriteLine(operationResult);
                    Pause();
                    break;

                case "7":
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

    //this shows the customer list first so the system can use the one the user picked instead of auto using one hardcoded customer
    static Customer SelectCustomer(List<Customer> customers)
    {
        while (true)
        {
            Console.WriteLine("\nSelect Customer by typing in the number:");

            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine((i + 1) + ".");
                Console.WriteLine(customers[i].ViewcustomerInfo());
            }

            Console.Write("Enter your choice or type back (b): ");
            string choice = Console.ReadLine();

            if (choice.Equals("back", StringComparison.OrdinalIgnoreCase) ||
                choice.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            int index;
            if (int.TryParse(choice, out index) && index >= 1 && index <= customers.Count)
            {
                return customers[index - 1];
            }

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    //this does the same thing for staff so there is a proper chosen staff member for the menu session
    static Staff SelectStaff(List<Staff> staffMembers)
    {
        while (true)
        {
            Console.WriteLine("\nSelect Staff Member by typing the number:");

            for (int i = 0; i < staffMembers.Count; i++)
            {
                Console.WriteLine((i + 1) + ".");
                Console.WriteLine(staffMembers[i].ViewStaffInfo());
            }

            Console.Write("Enter your choice or type back (b): ");
            string choice = Console.ReadLine();

            if (choice.Equals("back", StringComparison.OrdinalIgnoreCase) || 
                choice.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            int index;
            if (int.TryParse(choice, out index) && index >= 1 && index <= staffMembers.Count)
            {
                return staffMembers[index - 1];
            }

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    //this saves only the shipment fields we need into a text file line by line so they can be rebuilt later into Shipment objects
    static void SaveShipmentsToFile(List<Shipment> shipments, string shipmentFilePath)
    {
        List<string> lines = new List<string>();

        foreach (Shipment shipment in shipments)
        {
            string shipmentInfo = shipment.getShipmentInfo();
            string[] shipmentParts = ExtractShipmentData(shipmentInfo);

            lines.Add(shipmentParts[0] + "|" + shipmentParts[1] + "|" + shipmentParts[2] + "|" + shipmentParts[3]);
        }

        File.WriteAllLines(shipmentFilePath, lines);
    }

    //this reads the saved shipment text file and turns each line back into a Shipment object when the program starts
    static List<Shipment> LoadShipmentsFromFile(string shipmentFilePath)
    {
        List<Shipment> shipments = new List<Shipment>();

        if (!File.Exists(shipmentFilePath))
        {
            return shipments;
        }

        string[] lines = File.ReadAllLines(shipmentFilePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            if (parts.Length == 4)
            {
                Shipment shipment = new Shipment(parts[0], parts[1], parts[2], parts[3]);
                shipments.Add(shipment);
            }
        }

        return shipments;
    }

    //this pulls the four shipment values back out from the getShipmentInfo string so the current Shipment class can still be reused without changing it
    static string[] ExtractShipmentData(string shipmentInfo)
    {
        string[] lines = shipmentInfo.Split('\n');

        string shipmentID = lines[0].Replace("Shipment ID:", "").Trim();
        string shipmentStatus = lines[1].Replace("Shipment Status:", "").Trim();
        string currentLocation = lines[2].Replace("Current Location:", "").Trim();
        string deliveryStatus = lines[3].Replace("Delivery Status:", "").Trim();

        return new string[] { shipmentID, shipmentStatus, currentLocation, deliveryStatus };
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
                Console.WriteLine("Shipment ID cannot be left blank. Please try again or type back(b) to return.");
                continue;
            }

            if (enteredID.Equals("back", StringComparison.OrdinalIgnoreCase) || 
                enteredID.Equals("b", StringComparison.OrdinalIgnoreCase))
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

    //this registers the status listeners that should react when a shipment status changes
    //this supports the Observer pattern in the shipment update workflow without using assignment-style class names
    static void RegisterShipmentStatusListeners(Shipment shipment)
    {
        shipment.AttachStatusListener(new TrackingUpdateRecorder());
        shipment.AttachStatusListener(new CustomerStatusNotifier());
        shipment.AttachStatusListener(new StaffStatusNotifier());
    }

    //this just pauses the program so the user can read the output before the menu comes again
    static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}