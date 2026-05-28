using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MainfreightProject;


// MainfreightForm is the Windows Forms boundary layer for the Mainfreight system.
// The form handles user interaction only. Shipment workflows are delegated to
// ShipmentController, which then works with the repository, access control,
// Factory Method operation classes, and Observer/listener classes.
public class MainfreightForm : Form
{
    private readonly Color mainBlue = Color.FromArgb(0, 74, 141);
    private readonly Color darkBlue = Color.FromArgb(0, 43, 84);
    private readonly Color lightBlue = Color.FromArgb(232, 244, 255);

    private List<Customer> customers;
    private List<Staff> staffMembers;
    private List<Shipment> shipments;
    private Dictionary<Customer, Shipment> customerShipments;

    private string shipmentFilePath = "shipments.txt";

    private IShipmentRepo shipmentRepo;
    private AccessControlService accessControlService;
    private ShipmentOperationFactory shipmentOperationFactory;
    private ShipmentController shipmentController;

    private ComboBox customerComboBox;
    private ComboBox staffComboBox;
    private ComboBox staffShipmentComboBox;
    private ComboBox statusComboBox;
    private ComboBox recordsShipmentComboBox;
    private ComboBox operationStaffComboBox;
    private ComboBox operationShipmentComboBox;
    private ComboBox operationTypeComboBox;

    private TextBox newShipmentIDTextBox;
    private TextBox newLocationTextBox;
    private TextBox outputTextBox;

    public MainfreightForm()
    {
        Text = "Mainfreight Logistics System";
        Width = 1260;
        Height = 740;
        MinimumSize = new Size(1180 , 720);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;

        LoadDemoData();
        BuildFormLayout();
        RefreshShipmentSelectors();

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

        customerShipments = new Dictionary<Customer, Shipment>();

        if (customers.Count > 0 && shipments.Count > 0)
        {
            customerShipments[customers[0]] = shipments[0];
        }

        if (customers.Count > 1 && shipments.Count > 1)
        {
            customerShipments[customers[1]] = shipments[1];
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
        Panel sidebar = new Panel();
        sidebar.BackColor = darkBlue;
        sidebar.Location = new Point(0, 0);
        sidebar.Size = new Size(250, ClientSize.Height);
        sidebar.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
        Controls.Add(sidebar);

        Label brandMain = new Label();
        brandMain.Text = "MAIN";
        brandMain.ForeColor = Color.White;
        brandMain.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        brandMain.Location = new Point(25, 45);
        brandMain.Size = new Size(200, 40);
        sidebar.Controls.Add(brandMain);

        Label brandFreight = new Label();
        brandFreight.Text = "FREIGHT";
        brandFreight.ForeColor = Color.White;
        brandFreight.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        brandFreight.Location = new Point(25, 85);
        brandFreight.Size = new Size(210, 40);
        sidebar.Controls.Add(brandFreight);

        Label brandSubLabel = new Label();
        brandSubLabel.Text = "Logistics Management System";
        brandSubLabel.ForeColor = Color.FromArgb(210, 228, 246);
        brandSubLabel.Font = new Font("Segoe UI", 9);
        brandSubLabel.Location = new Point(28, 140);
        brandSubLabel.Size = new Size(195, 45);
        sidebar.Controls.Add(brandSubLabel);

        Label sidebarInfo = new Label();
        sidebarInfo.Text =
            "Phase II Prototype\n\n" +
            "GUI Layer\n" +
            "Controller Workflow\n" +
            "Factory Method\n" +
            "Observer Pattern";
        sidebarInfo.ForeColor = Color.FromArgb(220, 235, 250);
        sidebarInfo.Font = new Font("Segoe UI", 10);
        sidebarInfo.Location = new Point(28, 230);
        sidebarInfo.Size = new Size(190, 180);
        sidebar.Controls.Add(sidebarInfo);

        Label pageTitle = new Label();
        pageTitle.Text = "Mainfreight System Prototype";
        pageTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        pageTitle.ForeColor = darkBlue;
        pageTitle.Location = new Point(285, 30);
        pageTitle.Size = new Size(650, 40);
        Controls.Add(pageTitle);

        Label pageSubtitle = new Label();
        pageSubtitle.Text = "Customer tracking, staff status updates, shipment records, and department operation workflows.";
        pageSubtitle.Font = new Font("Segoe UI", 10);
        pageSubtitle.ForeColor = Color.DimGray;
        pageSubtitle.Location = new Point(288, 72);
        pageSubtitle.Size = new Size(760, 25);
        Controls.Add(pageSubtitle);

        TabControl tabControl = new TabControl();
        tabControl.Location = new Point(285, 125);
        tabControl.Size = new Size(620, 520);
        tabControl.Font = new Font("Segoe UI", 10);
        tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

        tabControl.TabPages.Add(BuildCustomerPortalTab());
        tabControl.TabPages.Add(BuildStaffOperationsTab());
        tabControl.TabPages.Add(BuildShipmentRecordsTab());
        tabControl.TabPages.Add(BuildDepartmentOperationsTab());

        Controls.Add(tabControl);

        Panel resultPanel = new Panel();
        resultPanel.BackColor = Color.FromArgb(247, 249, 252);
        resultPanel.Location = new Point(930, 125);
        resultPanel.Size = new Size(300, 520);
        resultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        resultPanel.BorderStyle = BorderStyle.FixedSingle;
        Controls.Add(resultPanel);

        Label resultTitle = new Label();
        resultTitle.Text = "Selected Result";
        resultTitle.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        resultTitle.ForeColor = darkBlue;
        resultTitle.Location = new Point(18, 18);
        resultTitle.Size = new Size(250, 32);
        resultPanel.Controls.Add(resultTitle);

        Label resultSubtitle = new Label();
        resultSubtitle.Text = "Workflow output and demo evidence will appear here.";
        resultSubtitle.Font = new Font("Segoe UI", 9);
        resultSubtitle.ForeColor = Color.DimGray;
        resultSubtitle.Location = new Point(20, 55);
        resultSubtitle.Size = new Size(255, 45);
        resultPanel.Controls.Add(resultSubtitle);

        outputTextBox = new TextBox();
        outputTextBox.Location = new Point(20, 110);
        outputTextBox.Size = new Size(255, 370);
        outputTextBox.Multiline = true;
        outputTextBox.ScrollBars = ScrollBars.Vertical;
        outputTextBox.ReadOnly = true;
        outputTextBox.BackColor = Color.White;
        outputTextBox.Font = new Font("Consolas", 9);
        outputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        resultPanel.Controls.Add(outputTextBox);
    }
    private TabPage BuildCustomerPortalTab()
    {
        TabPage tab = CreateTab("Customer Portal");

        AddHeading(tab, "Customer Portal", 25, 25);

        AddLabel(tab, "Select Customer:", 30, 80);
        customerComboBox = AddComboBox(tab, 170, 75, 260);
        customerComboBox.Items.Add("Lakshmi - Auckland");
        customerComboBox.Items.Add("Asha - Manukau");
        customerComboBox.SelectedIndex = 0;

        Button viewCustomerButton = AddButton(tab, "View Customer Details", 30, 130, 190);
        viewCustomerButton.Click += ViewCustomer_Click;

        Button trackShipmentButton = AddButton(tab, "Track Shipment", 240, 130, 160);
        trackShipmentButton.Click += TrackCustomerShipment_Click;

        Button historyButton = AddButton(tab, "View Tracking History", 420, 130, 190);
        historyButton.Click += CustomerTrackingHistory_Click;

        return tab;
    }

    private TabPage BuildStaffOperationsTab()
    {
        TabPage tab = CreateTab("Staff Operations");

        AddHeading(tab, "Staff Operations", 25, 25);

        AddLabel(tab, "Staff Member:", 30, 80);
        staffComboBox = AddComboBox(tab, 180, 75, 260);
        staffComboBox.Items.Add("Staff1 - Customer Service");
        staffComboBox.Items.Add("Staff2 - Operations");
        staffComboBox.SelectedIndex = 0;

        AddLabel(tab, "Shipment:", 30, 120);
        staffShipmentComboBox = AddComboBox(tab, 180, 115, 260);

        AddLabel(tab, "New Status:", 30, 160);
        statusComboBox = AddComboBox(tab, 180, 155, 260);
        statusComboBox.Items.AddRange(new string[]
        {
            "In Transit",
            "Out for delivery",
            "Delivered",
            "Delayed",
            "Returned"
        });
        statusComboBox.SelectedIndex = 0;

        Button updateButton = AddButton(tab, "Update Shipment Status", 470, 115, 220);
        updateButton.Click += UpdateShipmentStatus_Click;

        AddLabel(tab, "New Shipment ID:", 30, 235);
        newShipmentIDTextBox = AddTextBox(tab, 180, 230, 180);

        AddLabel(tab, "Current Location:", 30, 275);
        newLocationTextBox = AddTextBox(tab, 180, 270, 180);

        Button addButton = AddButton(tab, "Add New Shipment", 470, 235, 220);
        addButton.Click += AddShipment_Click;


        return tab;
    }

    private TabPage BuildShipmentRecordsTab()
    {
        TabPage tab = CreateTab("Shipment Records");

        AddHeading(tab, "Shipment Records", 25, 25);

        AddLabel(tab, "Shipment Record:", 30, 80);
        recordsShipmentComboBox = AddComboBox(tab, 180, 75, 260);

        Button refreshButton = AddButton(tab, "Refresh Records", 470, 75, 180);
        refreshButton.Click += RefreshRecords_Click;

        Button viewButton = AddButton(tab, "View Selected Shipment", 30, 130, 220);
        viewButton.Click += ViewSelectedRecord_Click;

        Button historyButton = AddButton(tab, "View Tracking History", 270, 130, 220);
        historyButton.Click += RecordTrackingHistory_Click;


        return tab;
    }

    private TabPage BuildDepartmentOperationsTab()
    {
        TabPage tab = CreateTab("Department Operations");

        AddHeading(tab, "Department Operations", 25, 25);

        AddLabel(tab, "Staff Member:", 30, 80);
        operationStaffComboBox = AddComboBox(tab, 200, 75, 260);
        operationStaffComboBox.Items.Add("Staff1 - Customer Service");
        operationStaffComboBox.Items.Add("Staff2 - Operations");
        operationStaffComboBox.SelectedIndex = 0;

        AddLabel(tab, "Shipment:", 30, 125);
        operationShipmentComboBox = AddComboBox(tab, 200, 120, 260);

        AddLabel(tab, "Operation Type:", 30, 170);
        operationTypeComboBox = AddComboBox(tab, 200, 165, 260);
        operationTypeComboBox.Items.AddRange(new string[]
        {
            "Transport Operation",
            "Warehouse Operation",
            "Customer Service Operation",
            "Returned Goods Operation"
        });
        operationTypeComboBox.SelectedIndex = 0;

        Button runButton = AddButton(tab, "Run Department Operation", 490, 120, 220);
        runButton.Click += RunDepartmentOperation_Click;


        return tab;
    }

    private TabPage CreateTab(string title)
    {
        TabPage tab = new TabPage(title);
        tab.BackColor = lightBlue;
        return tab;
    }

    private void AddHeading(Control parent, string text, int x, int y)
    {
        Label label = new Label();
        label.Text = text;
        label.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        label.ForeColor = darkBlue;
        label.AutoSize = true;
        label.Location = new Point(x, y);
        parent.Controls.Add(label);
    }

    private void AddLabel(Control parent, string text, int x, int y)
    {
        Label label = new Label();
        label.Text = text;
        label.Location = new Point(x, y);
        label.AutoSize = true;
        label.Font = new Font("Segoe UI", 10);
        parent.Controls.Add(label);
    }

    private ComboBox AddComboBox(Control parent, int x, int y, int width)
    {
        ComboBox comboBox = new ComboBox();
        comboBox.Location = new Point(x, y);
        comboBox.Width = width;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        parent.Controls.Add(comboBox);
        return comboBox;
    }

    private TextBox AddTextBox(Control parent, int x, int y, int width)
    {
        TextBox textBox = new TextBox();
        textBox.Location = new Point(x, y);
        textBox.Width = width;
        parent.Controls.Add(textBox);
        return textBox;
    }

    private Button AddButton(Control parent, string text, int x, int y, int width)
    {
        Button button = new Button();
        button.Text = text;
        button.Location = new Point(x, y);
        button.Size = new Size(width, 34);
        button.BackColor = mainBlue;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        parent.Controls.Add(button);
        return button;
    }


    private void RefreshShipmentSelectors()
    {
        ComboBox[] selectors =
        {
            staffShipmentComboBox,
            recordsShipmentComboBox,
            operationShipmentComboBox
        };

        foreach (ComboBox selector in selectors)
        {
            if (selector == null)
            {
                continue;
            }

            selector.Items.Clear();

            foreach (Shipment shipment in shipmentController.GetAllShipments())
            {
                selector.Items.Add(shipment.getShipmentID());
            }

            if (selector.Items.Count > 0)
            {
                selector.SelectedIndex = 0;
            }
        }
    }

    private Customer GetSelectedCustomer()
    {
        if (customerComboBox.SelectedIndex < 0 || customerComboBox.SelectedIndex >= customers.Count)
        {
            return null;
        }

        return customers[customerComboBox.SelectedIndex];
    }

    private Staff GetSelectedStaff(ComboBox comboBox)
    {
        if (comboBox.SelectedIndex < 0 || comboBox.SelectedIndex >= staffMembers.Count)
        {
            return null;
        }

        return staffMembers[comboBox.SelectedIndex];
    }

    private Shipment GetSelectedShipment(ComboBox comboBox)
    {
        if (comboBox.SelectedItem == null)
        {
            return null;
        }

        return shipmentController.FindShipmentByID(comboBox.SelectedItem.ToString());
    }

    private void ViewCustomer_Click(object sender, EventArgs e)
    {
        Customer customer = GetSelectedCustomer();

        if (customer == null)
        {
            DisplayMessage("Please select a customer.");
            return;
        }

        DisplayMessage(customer.ViewcustomerInfo());
    }

    private void TrackCustomerShipment_Click(object sender, EventArgs e)
    {
        Customer customer = GetSelectedCustomer();

        if (customer == null || !customerShipments.ContainsKey(customer))
        {
            DisplayMessage("No assigned shipment found for this customer.");
            return;
        }

        Shipment shipment = customerShipments[customer];

        string output = CaptureConsoleOutput(() =>
        {
            shipment.TrackShipment();
        });

        DisplayMessage(output);
    }

    private void CustomerTrackingHistory_Click(object sender, EventArgs e)
    {
        Customer customer = GetSelectedCustomer();

        if (customer == null || !customerShipments.ContainsKey(customer))
        {
            DisplayMessage("No assigned shipment found for this customer.");
            return;
        }

        Shipment shipment = customerShipments[customer];

        string output = CaptureConsoleOutput(() =>
        {
            shipment.viewTrackingHistory();
        });

        DisplayMessage(output);
    }

    private void UpdateShipmentStatus_Click(object sender, EventArgs e)
    {
        Staff staff = GetSelectedStaff(staffComboBox);
        Shipment shipment = GetSelectedShipment(staffShipmentComboBox);

        if (staff == null || shipment == null)
        {
            DisplayMessage("Please select staff and shipment.");
            return;
        }

        string output = CaptureConsoleOutput(() =>
        {
            string result = shipmentController.UpdateShipmentStatus(
                staff,
                shipment.getShipmentID(),
                statusComboBox.Text
            );

            Console.WriteLine(result);
        });

        RefreshShipmentSelectors();
        DisplayMessage(output + Environment.NewLine + shipment.getShipmentInfo());
    }

    private void AddShipment_Click(object sender, EventArgs e)
    {
        Staff staff = GetSelectedStaff(staffComboBox);

        if (staff == null)
        {
            DisplayMessage("Please select a staff member.");
            return;
        }

        string shipmentID = newShipmentIDTextBox.Text.Trim();
        string location = newLocationTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(shipmentID) || string.IsNullOrWhiteSpace(location))
        {
            DisplayMessage("Shipment ID and location cannot be blank.");
            return;
        }

        string result = shipmentController.AddNewShipment(
            staff,
            shipmentID,
            "In Transit",
            location,
            "Not Delivered"
        );

        RefreshShipmentSelectors();
        DisplayMessage(result);
    }

    private void RefreshRecords_Click(object sender, EventArgs e)
    {
        RefreshShipmentSelectors();
        DisplayMessage("Shipment records refreshed.");
    }

    private void ViewSelectedRecord_Click(object sender, EventArgs e)
    {
        Shipment shipment = GetSelectedShipment(recordsShipmentComboBox);

        if (shipment == null)
        {
            DisplayMessage("Please select a shipment.");
            return;
        }

        DisplayMessage(shipment.getShipmentInfo());
    }

    private void RecordTrackingHistory_Click(object sender, EventArgs e)
    {
        Shipment shipment = GetSelectedShipment(recordsShipmentComboBox);

        if (shipment == null)
        {
            DisplayMessage("Please select a shipment.");
            return;
        }

        string output = CaptureConsoleOutput(() =>
        {
            shipment.viewTrackingHistory();
        });

        DisplayMessage(output);
    }

    private void RunDepartmentOperation_Click(object sender, EventArgs e)
    {
        Staff staff = GetSelectedStaff(operationStaffComboBox);
        Shipment shipment = GetSelectedShipment(operationShipmentComboBox);

        if (staff == null || shipment == null)
        {
            DisplayMessage("Please select staff and shipment.");
            return;
        }

        string operationType = GetSelectedOperationType();

        string output = CaptureConsoleOutput(() =>
        {
            string result = shipmentController.RunDepartmentShipmentOperation(
                staff,
                shipment.getShipmentID(),
                operationType
            );

            Console.WriteLine(result);
        });

        DisplayMessage(output);
    }

    private string GetSelectedOperationType()
    {
        switch (operationTypeComboBox.SelectedIndex)
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
        outputTextBox.Text = string.IsNullOrWhiteSpace(message) ? "No output returned." : message;
    }

    private string CaptureConsoleOutput(Action action)
    {
        StringWriter writer = new StringWriter();
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetOut(writer);
            action();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        return writer.ToString();
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