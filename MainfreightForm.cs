using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MainfreightProject;

// MainfreightForm is the simple Windows Forms boundary layer for the prototype.
//
// Mainfreight context:
// This gives the system a more client-friendly interface while reusing the
// existing controller, repository, staff, shipment, tracking update, and
// design-pattern classes.
//
// UML-to-code mapping:
// MainfreightForm -> ShipmentController
// ShipmentController -> IShipmentRepo / AccessControlService / ShipmentOperationFactory
// ShipmentController -> Shipment
public class MainfreightForm : Form
{
    private List<Customer> customers;
    private List<Staff> staffMembers;
    private List<Shipment> shipments;

    private string shipmentFilePath = "shipments.txt";

    private IShipmentRepo shipmentRepo;
    private AccessControlService accessControlService;
    private ShipmentOperationFactory shipmentOperationFactory;
    private ShipmentController shipmentController;

    private ComboBox staffComboBox;
    private ListBox shipmentListBox;
    private TextBox shipmentIDTextBox;
    private TextBox locationTextBox;
    private ComboBox shipmentStatusComboBox;
    private ComboBox deliveryStatusComboBox;
    private ComboBox departmentOperationComboBox;
    private TextBox outputTextBox;

    public MainfreightForm()
    {
        Text = "Mainfreight Logistics System";
        Width = 950;
        Height = 650;
        StartPosition = FormStartPosition.CenterScreen;

        LoadDemoData();
        BuildFormLayout();
        RefreshShipmentList();
        DisplayMessage("Mainfreight Logistics System loaded successfully.");
    }

    private void LoadDemoData()
    {
        customers = new List<Customer>
        {
            new Customer("User1", "Lakshmi", "lakshmi@email.com", "Custom1", "0211236567", "Auckland"),
            new Customer("User3", "Asha", "asha@email.com", "Custom2", "0224567890", "Manukau")
        };

        staffMembers = new List<Staff>
        {
            new Staff("User2", "Nainika", "staff@email.com", "Staff1", "Customer Service"),
            new Staff("User4", "Riya", "riya@email.com", "Staff2", "Operations")
        };

        shipments = LoadShipmentsFromFile(shipmentFilePath);

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

        foreach (Shipment shipment in shipments)
        {
            RegisterShipmentStatusListeners(shipment);
        }

        if (shipments.Count > 0)
        {
            shipments[0].addTrackingUpdate(
                new TrackingUpdate("UPD001", DateTime.Now, "Shipment arrived at Auckland Depot.")
            );
        }

        if (shipments.Count > 1)
        {
            shipments[1].addTrackingUpdate(
                new TrackingUpdate("UPD002", DateTime.Now, "Shipment delivered successfully.")
            );
        }

        if (shipments.Count > 2)
        {
            shipments[2].addTrackingUpdate(
                new TrackingUpdate("UPD003", DateTime.Now, "Shipment delayed due to transport issue.")
            );
        }

        shipmentRepo = new ShipmentRepo(shipments, shipmentFilePath);
        accessControlService = new AccessControlService();
        shipmentOperationFactory = new ShipmentOperationFactory();

        shipmentController = new ShipmentController(
            shipmentRepo,
            accessControlService,
            shipmentOperationFactory
        );
    }

    private void BuildFormLayout()
    {
        Label titleLabel = new Label();
        titleLabel.Text = "Mainfreight Logistics System";
        titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        titleLabel.AutoSize = true;
        titleLabel.Location = new Point(25, 20);
        Controls.Add(titleLabel);


        Label staffLabel = new Label();
        staffLabel.Text = "Current Staff Member:";
        staffLabel.Location = new Point(30, 95);
        staffLabel.AutoSize = true;
        Controls.Add(staffLabel);

        staffComboBox = new ComboBox();
        staffComboBox.Location = new Point(160, 90);
        staffComboBox.Width = 250;
        staffComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        foreach (Staff staff in staffMembers)
        {
            staffComboBox.Items.Add(staff.ViewStaffInfo().Split('\n')[0].Replace("Name:", "").Trim());
        }

        if (staffComboBox.Items.Count > 0)
        {
            staffComboBox.SelectedIndex = 0;
        }

        Controls.Add(staffComboBox);

        Label listLabel = new Label();
        listLabel.Text = "Shipment Records:";
        listLabel.Location = new Point(30, 135);
        listLabel.AutoSize = true;
        Controls.Add(listLabel);

        shipmentListBox = new ListBox();
        shipmentListBox.Location = new Point(30, 160);
        shipmentListBox.Width = 380;
        shipmentListBox.Height = 220;
        shipmentListBox.SelectedIndexChanged += ShipmentListBox_SelectedIndexChanged;
        Controls.Add(shipmentListBox);

        Button refreshButton = new Button();
        refreshButton.Text = "Refresh Shipments";
        refreshButton.Location = new Point(30, 395);
        refreshButton.Width = 180;
        refreshButton.Click += RefreshButton_Click;
        Controls.Add(refreshButton);

        Button trackingButton = new Button();
        trackingButton.Text = "View Tracking History";
        trackingButton.Location = new Point(230, 395);
        trackingButton.Width = 180;
        trackingButton.Click += TrackingButton_Click;
        Controls.Add(trackingButton);

        Label detailsLabel = new Label();
        detailsLabel.Text = "Shipment Details / Actions:";
        detailsLabel.Location = new Point(450, 95);
        detailsLabel.AutoSize = true;
        Controls.Add(detailsLabel);

        Label shipmentIDLabel = new Label();
        shipmentIDLabel.Text = "Shipment ID:";
        shipmentIDLabel.Location = new Point(450, 130);
        shipmentIDLabel.AutoSize = true;
        Controls.Add(shipmentIDLabel);

        shipmentIDTextBox = new TextBox();
        shipmentIDTextBox.Location = new Point(590, 125);
        shipmentIDTextBox.Width = 250;
        Controls.Add(shipmentIDTextBox);

        Label locationLabel = new Label();
        locationLabel.Text = "Current Location:";
        locationLabel.Location = new Point(450, 170);
        locationLabel.AutoSize = true;
        Controls.Add(locationLabel);

        locationTextBox = new TextBox();
        locationTextBox.Location = new Point(590, 165);
        locationTextBox.Width = 250;
        Controls.Add(locationTextBox);

        Label statusLabel = new Label();
        statusLabel.Text = "Shipment Status:";
        statusLabel.Location = new Point(450, 210);
        statusLabel.AutoSize = true;
        Controls.Add(statusLabel);

        shipmentStatusComboBox = new ComboBox();
        shipmentStatusComboBox.Location = new Point(590, 205);
        shipmentStatusComboBox.Width = 250;
        shipmentStatusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        shipmentStatusComboBox.Items.AddRange(new string[]
        {
            "In Transit",
            "Out for delivery",
            "Delivered",
            "Delayed",
            "Returned"
        });
        shipmentStatusComboBox.SelectedIndex = 0;
        Controls.Add(shipmentStatusComboBox);

        Label deliveryLabel = new Label();
        deliveryLabel.Text = "Delivery Status:";
        deliveryLabel.Location = new Point(450, 250);
        deliveryLabel.AutoSize = true;
        Controls.Add(deliveryLabel);

        deliveryStatusComboBox = new ComboBox();
        deliveryStatusComboBox.Location = new Point(590, 245);
        deliveryStatusComboBox.Width = 250;
        deliveryStatusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        deliveryStatusComboBox.Items.AddRange(new string[]
        {
            "Delivered",
            "Not Delivered",
            "Returning"
        });
        deliveryStatusComboBox.SelectedIndex = 1;
        Controls.Add(deliveryStatusComboBox);

        Button updateButton = new Button();
        updateButton.Text = "Update Selected Shipment Status";
        updateButton.Location = new Point(450, 290);
        updateButton.Width = 390;
        updateButton.Click += UpdateButton_Click;
        Controls.Add(updateButton);

        Button addButton = new Button();
        addButton.Text = "Add New Shipment";
        addButton.Location = new Point(450, 330);
        addButton.Width = 390;
        addButton.Click += AddButton_Click;
        Controls.Add(addButton);

        Label operationLabel = new Label();
        operationLabel.Text = "Department Operation:";
        operationLabel.Location = new Point(450, 375);
        operationLabel.AutoSize = true;
        Controls.Add(operationLabel);

        departmentOperationComboBox = new ComboBox();
        departmentOperationComboBox.Location = new Point(590, 370);
        departmentOperationComboBox.Width = 250;
        departmentOperationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        departmentOperationComboBox.Items.AddRange(new string[]
        {
            "Transport Operation",
            "Warehouse Operation",
            "Customer Service Operation",
            "Returned Goods Operation"
        });
        departmentOperationComboBox.SelectedIndex = 0;
        Controls.Add(departmentOperationComboBox);

        Button operationButton = new Button();
        operationButton.Text = "Run Department Operation";
        operationButton.Location = new Point(450, 410);
        operationButton.Width = 390;
        operationButton.Click += OperationButton_Click;
        Controls.Add(operationButton);

        Label outputLabel = new Label();
        outputLabel.Text = "System Output:";
        outputLabel.Location = new Point(30, 445);
        outputLabel.AutoSize = true;
        Controls.Add(outputLabel);

        outputTextBox = new TextBox();
        outputTextBox.Location = new Point(30, 470);
        outputTextBox.Width = 810;
        outputTextBox.Height = 110;
        outputTextBox.Multiline = true;
        outputTextBox.ScrollBars = ScrollBars.Vertical;
        outputTextBox.ReadOnly = true;
        Controls.Add(outputTextBox);
    }

    private void RefreshShipmentList()
    {
        shipmentListBox.Items.Clear();

        foreach (Shipment shipment in shipmentController.GetAllShipments())
        {
            shipmentListBox.Items.Add(shipment.getShipmentID());
        }
    }

    private void ShipmentListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        Shipment selectedShipment = GetSelectedShipment();

        if (selectedShipment == null)
        {
            return;
        }

        string[] shipmentParts = ExtractShipmentData(selectedShipment.getShipmentInfo());

        shipmentIDTextBox.Text = shipmentParts[0];
        shipmentStatusComboBox.SelectedItem = shipmentParts[1];
        locationTextBox.Text = shipmentParts[2];
        deliveryStatusComboBox.SelectedItem = shipmentParts[3];

        DisplayMessage(selectedShipment.getShipmentInfo());
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        RefreshShipmentList();
        DisplayMessage("Shipment list refreshed.");
    }

    private void TrackingButton_Click(object sender, EventArgs e)
    {
        Shipment selectedShipment = GetSelectedShipment();

        if (selectedShipment == null)
        {
            DisplayMessage("Please select a shipment first.");
            return;
        }

        DisplayMessage("Tracking history is shown in the console output for now.\r\nSelected shipment: " + selectedShipment.getShipmentID());
        selectedShipment.viewTrackingHistory();
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
        Shipment selectedShipment = GetSelectedShipment();

        if (selectedShipment == null)
        {
            DisplayMessage("Please select a shipment to update.");
            return;
        }

        Staff selectedStaff = GetSelectedStaff();

        if (selectedStaff == null)
        {
            DisplayMessage("Please select a staff member.");
            return;
        }

        string newStatus = shipmentStatusComboBox.Text;

        string result = shipmentController.UpdateShipmentStatus(
            selectedStaff,
            selectedShipment.getShipmentID(),
            newStatus
        );

        RefreshShipmentList();
        DisplayMessage(result + "\r\n\r\n" + selectedShipment.getShipmentInfo());
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        Staff selectedStaff = GetSelectedStaff();

        if (selectedStaff == null)
        {
            DisplayMessage("Please select a staff member.");
            return;
        }

        string newShipmentID = shipmentIDTextBox.Text.Trim();
        string newLocation = locationTextBox.Text.Trim();
        string newShipmentStatus = shipmentStatusComboBox.Text;
        string newDeliveryStatus = deliveryStatusComboBox.Text;

        if (string.IsNullOrWhiteSpace(newShipmentID) || string.IsNullOrWhiteSpace(newLocation))
        {
            DisplayMessage("Shipment ID and current location cannot be blank.");
            return;
        }

        string result = shipmentController.AddNewShipment(
            selectedStaff,
            newShipmentID,
            newShipmentStatus,
            newLocation,
            newDeliveryStatus
        );

        RefreshShipmentList();
        DisplayMessage(result);
    }

    private void OperationButton_Click(object sender, EventArgs e)
    {
        Shipment selectedShipment = GetSelectedShipment();

        if (selectedShipment == null)
        {
            DisplayMessage("Please select a shipment first.");
            return;
        }

        Staff selectedStaff = GetSelectedStaff();

        if (selectedStaff == null)
        {
            DisplayMessage("Please select a staff member.");
            return;
        }

        string operationType = GetSelectedOperationType();

        string result = shipmentController.RunDepartmentShipmentOperation(
            selectedStaff,
            selectedShipment.getShipmentID(),
            operationType
        );

        DisplayMessage(result + "\r\n\r\nOperation was executed using the department operation workflow.");
    }

    private Shipment GetSelectedShipment()
    {
        if (shipmentListBox.SelectedItem == null)
        {
            return null;
        }

        string selectedShipmentID = shipmentListBox.SelectedItem.ToString();

        return shipmentController.FindShipmentByID(selectedShipmentID);
    }

    private Staff GetSelectedStaff()
    {
        if (staffComboBox.SelectedIndex < 0 || staffComboBox.SelectedIndex >= staffMembers.Count)
        {
            return null;
        }

        return staffMembers[staffComboBox.SelectedIndex];
    }

    private string GetSelectedOperationType()
    {
        switch (departmentOperationComboBox.SelectedIndex)
        {
            case 0:
                return "transport";

            case 1:
                return "warehouse";

            case 2:
                return "customerservice";

            case 3:
                return "return";

            default:
                return "transport";
        }
    }

    private void DisplayMessage(string message)
    {
        outputTextBox.Text = message;
    }

    private void RegisterShipmentStatusListeners(Shipment shipment)
    {
        shipment.AttachStatusListener(new TrackingUpdateRecorder());
        shipment.AttachStatusListener(new CustomerStatusNotifier());
        shipment.AttachStatusListener(new StaffStatusNotifier());
    }

    private void SaveShipmentsToFile(List<Shipment> shipments, string shipmentFilePath)
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

    private List<Shipment> LoadShipmentsFromFile(string shipmentFilePath)
    {
        List<Shipment> loadedShipments = new List<Shipment>();

        if (!File.Exists(shipmentFilePath))
        {
            return loadedShipments;
        }

        string[] lines = File.ReadAllLines(shipmentFilePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            if (parts.Length == 4)
            {
                Shipment shipment = new Shipment(parts[0], parts[1], parts[2], parts[3]);
                loadedShipments.Add(shipment);
            }
        }

        return loadedShipments;
    }

    private string[] ExtractShipmentData(string shipmentInfo)
    {
        string[] lines = shipmentInfo.Split('\n');

        string shipmentID = lines[0].Replace("Shipment ID:", "").Trim();
        string shipmentStatus = lines[1].Replace("Shipment Status:", "").Trim();
        string currentLocation = lines[2].Replace("Current Location:", "").Trim();
        string deliveryStatus = lines[3].Replace("Delivery Status:", "").Trim();

        return new string[] { shipmentID, shipmentStatus, currentLocation, deliveryStatus };
    }
}