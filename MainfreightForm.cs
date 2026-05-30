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
// MainfreightForm is the Windows Forms boundary layer for the logistics system.
// It opens with public customer shipment tracking, then allows Staff/Admin users to authenticate for internal dashboard access.
// Shipment workflows are delegated to ShipmentController so the form focuses on user interaction, validation, and display.


public class MainfreightForm : Form
{
    private readonly Color mainfreightBlue = Color.FromArgb(0, 73, 141);
    private readonly Color navyBlue = Color.FromArgb(0, 36, 75);
    private readonly Color softBlue = Color.FromArgb(232, 244, 255);
    private readonly Color palePanel = Color.FromArgb(246, 250, 253);
    private readonly Color textDark = Color.FromArgb(20, 43, 70);

    private readonly string shipmentFilePath = "shipments.txt";
    private readonly string accountFilePath = "accounts.txt";

    private List<Customer> customers = new List<Customer>();
    private List<Staff> staffMembers = new List<Staff>();
    private List<string> staffDisplayNames = new List<string>();
    private List<Shipment> shipments = new List<Shipment>();
    private Dictionary<Customer, Shipment> customerShipments = new Dictionary<Customer, Shipment>();

    private IShipmentRepo shipmentRepo = null!;
    private AccessControlService accessControlService = null!;
    private ShipmentOperationFactory shipmentOperationFactory = null!;
    private ShipmentController shipmentController = null!;
    private AuthenticationService authenticationService = null!;

    private UserAccount? currentAccount;
    private string selectedAccessType = "Staff";

    private Panel publicPanel = null!;
    private Panel loginPanel = null!;
    private Panel dashboardPanel = null!;

    private ComboBox publicShipmentComboBox = null!;
    private RichTextBox publicSummaryBox = null!;
    private RichTextBox publicHistoryBox = null!;

    private TextBox usernameTextBox = null!;
    private TextBox passwordTextBox = null!;
    private Label loginMessageLabel = null!;

    private Label dashboardUserLabel = null!;
    private TabControl staffTabControl = null!;

    private ComboBox staffComboBox = null!;
    private ComboBox staffShipmentComboBox = null!;
    private ComboBox statusComboBox = null!;
    private ComboBox updateLocationComboBox = null!;
    private TextBox newShipmentIDTextBox = null!;
    private ComboBox newLocationComboBox = null!;
    private RichTextBox staffResultBox = null!;

    private ComboBox recordsShipmentComboBox = null!;
    private RichTextBox recordsResultBox = null!;

    private ComboBox operationStaffComboBox = null!;
    private ComboBox operationShipmentComboBox = null!;
    private ComboBox operationTypeComboBox = null!;
    private RichTextBox operationResultBox = null!;

    private ListBox adminStaffListBox = null!;
    private TextBox adminStaffNameTextBox = null!;
    private TextBox adminStaffEmailTextBox = null!;
    private ComboBox adminStaffDepartmentComboBox = null!;
    private RichTextBox adminResultBox = null!;

    public MainfreightForm()
    {
        Text = "Mainfreight Logistics Management System";
        Width = 1460;
        Height = 920;
        MinimumSize = new Size(1360, 860);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        AutoScaleMode = AutoScaleMode.Font;

        LoadDemoData();
        authenticationService = new AuthenticationService(accountFilePath);

        BuildBaseLayout();
        ShowPublicTrackingPanel();
    }

    private void LoadDemoData()
    {
        customers = new List<Customer>
        {
            new Customer("User1", "Lakshmi", "lakshmi@email.com", "Custom1", "0211234567", "Auckland"),
            new Customer("User3", "Asha", "asha@email.com", "Custom2", "0224567890", "Manukau")
        };

        staffMembers = new List<Staff>
        {
            new Staff("User2", "Nainika", "staff@email.com", "Staff1", "Customer Service"),
            new Staff("User4", "Riya", "riya@email.com", "Staff2", "Operations")
        };

        staffDisplayNames = new List<string>
        {
            "Nainika - Customer Service",
            "Riya - Operations"
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
            shipments[0].addTrackingUpdate(new TrackingUpdate("UPD001", DateTime.Now, "Shipment arrived at Auckland Depot."));
        }

        if (shipments.Count > 1)
        {
            shipments[1].addTrackingUpdate(new TrackingUpdate("UPD002", DateTime.Now, "Shipment delivered successfully."));
        }

        if (shipments.Count > 2)
        {
            shipments[2].addTrackingUpdate(new TrackingUpdate("UPD003", DateTime.Now, "Shipment delayed due to transport issue."));
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
        shipmentController = new ShipmentController(shipmentRepo, accessControlService, shipmentOperationFactory);
    }

    private void BuildBaseLayout()
    {
        Controls.Clear();

        TableLayoutPanel rootLayout = new TableLayoutPanel();
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.RowCount = 2;
        rootLayout.ColumnCount = 1;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(rootLayout);

        Panel headerPanel = new Panel();
        headerPanel.Dock = DockStyle.Fill;
        headerPanel.BackColor = navyBlue;
        rootLayout.Controls.Add(headerPanel, 0, 0);

        Label logoLabel = new Label();
        logoLabel.Text = "MAINFREIGHT";
        logoLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        logoLabel.ForeColor = Color.White;
        logoLabel.Location = new Point(32, 13);
        logoLabel.Size = new Size(275, 40);
        headerPanel.Controls.Add(logoLabel);

        Label headerSubtitle = new Label();
        headerSubtitle.Text = "Logistics Management System";
        headerSubtitle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
        headerSubtitle.ForeColor = Color.FromArgb(215, 232, 248);
        headerSubtitle.Location = new Point(315, 18);
        headerSubtitle.Size = new Size(450, 32);
        headerSubtitle.TextAlign = ContentAlignment.MiddleLeft;
        headerSubtitle.UseCompatibleTextRendering = true;
        headerPanel.Controls.Add(headerSubtitle);

        Panel contentPanel = new Panel();
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.BackColor = Color.White;
        rootLayout.Controls.Add(contentPanel, 0, 1);

        publicPanel = CreateContentPanel(contentPanel);
        loginPanel = CreateContentPanel(contentPanel);
        dashboardPanel = CreateContentPanel(contentPanel);

        BuildPublicPanel();
        BuildLoginPanel();
        BuildDashboardPanel();
    }

    private Panel CreateContentPanel(Control parent)
    {
        Panel panel = new Panel();
        panel.Dock = DockStyle.Fill;
        panel.BackColor = Color.White;
        parent.Controls.Add(panel);
        return panel;
    }

    private void BuildPublicPanel()
    {
        publicPanel.Controls.Clear();

        AddTitle(publicPanel, "Track Shipment", 60, 45, 520);

        Label intro = AddPlainText(publicPanel,
            "Enter a shipment reference to view the current delivery status.",
            64, 100, 900);
        intro.Height = 34;
        intro.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        intro.ForeColor = mainfreightBlue;

        AddSectionHeading(publicPanel, "Customer Tracking", 65, 185);

        Label shipmentIdLabel = AddLabel(publicPanel, "Shipment ID:", 68, 248);
        shipmentIdLabel.Size = new Size(115, 30);
        shipmentIdLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        shipmentIdLabel.ForeColor = mainfreightBlue;

        publicShipmentComboBox = AddComboBox(publicPanel, 190, 244, 230);
        publicShipmentComboBox.Items.Clear();

        foreach (Shipment shipment in shipmentController.GetAllShipments())
        {
            publicShipmentComboBox.Items.Add(shipment.getShipmentID());
        }

        if (publicShipmentComboBox.Items.Count > 0)
        {
            publicShipmentComboBox.SelectedIndex = 0;
        }

        Button trackButton = AddPrimaryButton(publicPanel, "Track Shipment", 445, 242, 165);
        trackButton.Click += PublicTrackShipment_Click;

        AddSectionHeading(publicPanel, "Shipment Summary", 65, 330);
        publicSummaryBox = AddResultBox(publicPanel, 68, 380, 560, 185);
        SetResultMessage(publicSummaryBox, "Select a shipment and click Track Shipment.");

        Label trackingHeading = AddSectionHeadingLabel(publicPanel, "Tracking History", 650, 330, 560);
        trackingHeading.BringToFront();

        publicHistoryBox = AddResultBox(publicPanel, 653, 380, 560, 185);
        SetResultMessage(publicHistoryBox, "Tracking updates will appear here.");

        AddSectionHeading(publicPanel, "Internal Access", 65, 640);

        Label accessText = AddPlainText(publicPanel,
            "Please sign in with your access details to view further.",
            68, 685, 700);
        accessText.Height = 32;
        accessText.Font = new Font("Segoe UI", 10, FontStyle.Regular);

        Button staffButton = AddSecondaryButton(publicPanel, "Staff Access", 820, 675, 150);
        staffButton.Click += (sender, e) => ShowLoginPanel("Staff");

        Button adminButton = AddSecondaryButton(publicPanel, "Admin Access", 990, 675, 150);
        adminButton.Click += (sender, e) => ShowLoginPanel("Admin");
    }

    private void BuildLoginPanel()
    {
        loginPanel.Controls.Clear();

        AddTitle(loginPanel, "Internal Access Login", 420, 95, 620);

        Label intro = AddPlainText(loginPanel,
            "Please sign in with your access details to view further.",
            422, 150, 560);
        intro.Height = 38;
        intro.Font = new Font("Segoe UI", 10, FontStyle.Regular);

        AddLabel(loginPanel, "Username:", 425, 235);
        usernameTextBox = AddTextBox(loginPanel, 425, 268, 360);
        usernameTextBox.TextChanged += (sender, e) => ResetLoginInputStyles();
        
        AddLabel(loginPanel, "Password:", 425, 328);
        passwordTextBox = AddTextBox(loginPanel, 425, 361, 360);
        passwordTextBox.PasswordChar = '*';
        passwordTextBox.TextChanged += (sender, e) => ResetLoginInputStyles();

        loginMessageLabel = AddPlainText(loginPanel, "", 425, 425, 520);
        loginMessageLabel.Height = 60;
        loginMessageLabel.BackColor = palePanel;
        loginMessageLabel.Padding = new Padding(12);
        loginMessageLabel.ForeColor = Color.Firebrick;

        Button loginButton = AddPrimaryButton(loginPanel, "Login", 425, 515, 165);
        loginButton.Click += LoginButton_Click;

        Button backButton = AddSecondaryButton(loginPanel, "Back to Customer Page", 610, 515, 220);
        backButton.Click += (sender, e) => ShowPublicTrackingPanel();
    }

    private void BuildDashboardPanel()
    {
        dashboardPanel.Controls.Clear();

        AddTitle(dashboardPanel, "Internal Operations", 35, 38, 650);

        dashboardUserLabel = AddPlainText(dashboardPanel, "Not logged in.", 42, 98, 600);
        dashboardUserLabel.Height = 36;
        dashboardUserLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dashboardUserLabel.ForeColor = Color.DimGray;

        Button logoutButton = AddSecondaryButton(dashboardPanel, "Back to Customer Page", 1100, 45, 220);
        logoutButton.Click += (sender, e) =>
        {
            currentAccount = null;
            ShowPublicTrackingPanel();
        };

        staffTabControl = new TabControl();
        staffTabControl.Location = new Point(35, 145);
        staffTabControl.Size = new Size(1325, 655);
        staffTabControl.Font = new Font("Segoe UI", 10);
        dashboardPanel.Controls.Add(staffTabControl);

        staffTabControl.TabPages.Add(BuildStaffOperationsTab());
        staffTabControl.TabPages.Add(BuildShipmentRecordsTab());
        staffTabControl.TabPages.Add(BuildDepartmentOperationsTab());
    }

    private TabPage BuildStaffOperationsTab()
    {
        TabPage tab = CreateTabPage("Staff Operations");

        AddSectionHeading(tab, "Update Shipment Status", 45, 35);

        AddLabel(tab, "Staff Member:", 50, 98);
        staffComboBox = AddComboBox(tab, 240, 93, 290);
        LoadStaffOptions(staffComboBox);

        AddLabel(tab, "Shipment:", 50, 148);
        staffShipmentComboBox = AddComboBox(tab, 240, 143, 290);

        AddLabel(tab, "New Status:", 50, 198);
        statusComboBox = AddComboBox(tab, 240, 193, 290);
        statusComboBox.Items.AddRange(new string[]
        {
            "In Transit",
            "Out for delivery",
            "Delivered",
            "Delayed",
            "Returned"
        });
        statusComboBox.SelectedIndex = 0;
        statusComboBox.SelectedIndexChanged += (sender, e) => UpdateLocationOptionsForStatus();

        Label currentLocationLabel = AddLabel(tab, "Current Location:", 50, 248);
        currentLocationLabel.Size = new Size(170, 30);
        updateLocationComboBox = AddComboBox(tab, 240, 243, 290);
        updateLocationComboBox.Items.AddRange(GetLocationOptions());
        updateLocationComboBox.SelectedIndex = 0;
        UpdateLocationOptionsForStatus();

        Button updateButton = AddPrimaryButton(tab, "Update Status", 570, 168, 170);
        updateButton.Click += UpdateShipmentStatus_Click;

        AddSectionHeading(tab, "Add New Shipment", 45, 330);

        AddLabel(tab, "Shipment ID:", 50, 390);
        newShipmentIDTextBox = AddTextBox(tab, 240, 385, 220);

        AddLabel(tab, "Location:", 50, 440);
        newLocationComboBox = AddComboBox(tab, 240, 435, 290);
        newLocationComboBox.Items.AddRange(GetDepotLocationOptions());
        newLocationComboBox.SelectedIndex = 0;

        Button addButton = AddPrimaryButton(tab, "Add Shipment", 570, 385, 170);
        addButton.Click += AddShipment_Click;

        AddSectionHeading(tab, "Operation Result", 830, 68);
        staffResultBox = AddResultBox(tab, 835, 120, 430, 300);
        SetResultMessage(staffResultBox, "Ready for staff shipment operations.");

        return tab;
    }

    private TabPage BuildShipmentRecordsTab()
    {
        TabPage tab = CreateTabPage("Shipment Records");

        AddSectionHeading(tab, "Shipment Record Lookup", 45, 35);

        AddLabel(tab, "Shipment:", 50, 100);
        recordsShipmentComboBox = AddComboBox(tab, 180, 95, 300);

        Button refreshButton = AddPrimaryButton(tab, "Refresh", 520, 95, 145);
        refreshButton.Click += RefreshRecords_Click;

        Button viewButton = AddSecondaryButton(tab, "View Details", 50, 165, 160);
        viewButton.Click += ViewSelectedRecord_Click;

        Button historyButton = AddSecondaryButton(tab, "View Tracking", 230, 165, 165);
        historyButton.Click += RecordTrackingHistory_Click;

        AddSectionHeading(tab, "Record Details", 50, 260);
        recordsResultBox = AddResultBox(tab, 55, 310, 1120, 250);
        SetResultMessage(recordsResultBox, "Select a shipment record to view details.");

        return tab;
    }

    private TabPage BuildDepartmentOperationsTab()
    {
        TabPage tab = CreateTabPage("Department Processing");

        AddSectionHeading(tab, "Record Department Processing", 45, 35);

        AddLabel(tab, "Staff Member:", 50, 100);
        operationStaffComboBox = AddComboBox(tab, 240, 95, 310);
        LoadStaffOptions(operationStaffComboBox);
        operationStaffComboBox.SelectedIndexChanged += (sender, e) => UpdateAllowedDepartmentProcessing();

        AddLabel(tab, "Shipment:", 50, 150);
        operationShipmentComboBox = AddComboBox(tab, 240, 145, 310);
        operationShipmentComboBox.SelectedIndexChanged += (sender, e) => UpdateAllowedDepartmentProcessing();

        AddLabel(tab, "Processing Type:", 50, 200);
        operationTypeComboBox = AddComboBox(tab, 240, 195, 310);

        Button runButton = AddPrimaryButton(tab, "Record Processing", 580, 145, 180);
        runButton.Click += RunDepartmentOperation_Click;

        AddSectionHeading(tab, "Processing Result", 50, 310);
        operationResultBox = AddResultBox(tab, 55, 360, 1120, 205);
        SetResultMessage(operationResultBox, "Select a staff member and shipment to record department processing.");

        UpdateAllowedDepartmentProcessing();

        return tab;
    }

    private TabPage BuildStaffManagementTab()
    {
        TabPage tab = CreateTabPage("Staff Management");

        AddSectionHeading(tab, "Staff Management", 45, 35);

        AddLabel(tab, "Current Staff Members:", 50, 95);

        adminStaffListBox = new ListBox();
        adminStaffListBox.Location = new Point(50, 130);
        adminStaffListBox.Size = new Size(380, 320);
        adminStaffListBox.Font = new Font("Segoe UI", 10);
        adminStaffListBox.BorderStyle = BorderStyle.None;
        adminStaffListBox.BackColor = palePanel;
        tab.Controls.Add(adminStaffListBox);

        AddLabel(tab, "Staff Name:", 500, 100);
        adminStaffNameTextBox = AddTextBox(tab, 650, 95, 280);

        AddLabel(tab, "Staff Email:", 500, 148);
        adminStaffEmailTextBox = AddTextBox(tab, 650, 143, 280);

        AddLabel(tab, "Department:", 500, 196);
        adminStaffDepartmentComboBox = AddComboBox(tab, 650, 191, 280);
        adminStaffDepartmentComboBox.Items.AddRange(new string[]
        {
            "Customer Service",
            "Operations",
            "Transport",
            "Warehouse",
            "Returned Goods",
            "Air and Ocean",
            "IT"
        });
        adminStaffDepartmentComboBox.SelectedIndex = 0;

        Button addStaffButton = AddPrimaryButton(tab, "Add Staff", 500, 255, 160);
        addStaffButton.Click += AddStaffMember_Click;

        Button removeStaffButton = AddSecondaryButton(tab, "Remove Selected", 680, 255, 180);
        removeStaffButton.Click += RemoveStaffMember_Click;

        AddSectionHeading(tab, "Admin Result", 500, 340);
        adminResultBox = AddResultBox(tab, 505, 390, 620, 150);
        SetResultMessage(adminResultBox, "Admin staff changes will appear here.");

        RefreshAdminStaffList();

        return tab;
    }

    private void ShowPublicTrackingPanel()
    {
        publicPanel.Visible = true;
        loginPanel.Visible = false;
        dashboardPanel.Visible = false;

        usernameTextBox.Clear();
        passwordTextBox.Clear();

        if (loginMessageLabel != null)
        {
            loginMessageLabel.Text = "";
        }
    }

    private void ShowLoginPanel(string accessType)
    {
        selectedAccessType = accessType;

        publicPanel.Visible = false;
        loginPanel.Visible = true;
        dashboardPanel.Visible = false;

        usernameTextBox.Clear();
        passwordTextBox.Clear();
        loginMessageLabel.Text = accessType + " access selected. Please sign in.";
        usernameTextBox.Focus();
    }

    private void ShowDashboardPanel()
    {
        publicPanel.Visible = false;
        loginPanel.Visible = false;
        dashboardPanel.Visible = true;

        if (currentAccount != null)
        {
            dashboardUserLabel.Text = "Logged in as " + currentAccount.Username + " (" + currentAccount.Role + ")";
        }

        RefreshShipmentSelectors();
        UpdateDashboardTabsForCurrentUser();
        UpdateAllowedDepartmentProcessing();

        SetResultMessage(staffResultBox, "Ready for staff shipment operations.");
        SetResultMessage(recordsResultBox, "Select a shipment record to view details.");
        SetResultMessage(operationResultBox, "Select a staff member and shipment to record department processing.");
    }

    private void UpdateDashboardTabsForCurrentUser()
    {
        for (int i = staffTabControl.TabPages.Count - 1; i >= 0; i--)
        {
            if (staffTabControl.TabPages[i].Text == "Staff Management")
            {
                staffTabControl.TabPages.RemoveAt(i);
            }
        }

        if (currentAccount != null && currentAccount.Role == UserRole.Admin)
        {
            staffTabControl.TabPages.Add(BuildStaffManagementTab());
        }
    }

    private void PublicTrackShipment_Click(object? sender, EventArgs e)
    {
        string shipmentID = publicShipmentComboBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(shipmentID))
        {
            MessageBox.Show("Please select a shipment ID.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Shipment shipment = shipmentController.FindShipmentByID(shipmentID);

        if (shipment == null)
        {
            MessageBox.Show("Shipment not found. Please check the shipment ID and try again.",
                "Shipment Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ShowShipmentSummary(publicSummaryBox, shipment);
        ShowTrackingHistory(publicHistoryBox, shipment);
    }

    private void LoginButton_Click(object? sender, EventArgs e)
{
    ResetLoginInputStyles();

    string username = usernameTextBox.Text.Trim();
    string password = passwordTextBox.Text.Trim();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            HighlightLoginBox(usernameTextBox);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            HighlightLoginBox(passwordTextBox);
        }

        MessageBox.Show("Please enter both username and password.", "Login Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    UserAccount account = authenticationService.Authenticate(username, password);

    if (account == null)
    {
        bool usernameExists = UsernameExistsInAccountFile(username);

        if (usernameExists)
        {
            HighlightLoginBox(passwordTextBox);
            loginMessageLabel.Text = "Password is incorrect";
        }
        else
        {
            HighlightLoginBox(usernameTextBox);
            HighlightLoginBox(passwordTextBox);
            loginMessageLabel.Text = "Invalid login details. Please check the highlighted fields.";
        }

        MessageBox.Show("Invalid login details or inactive account.", "Login Failed",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    if (selectedAccessType == "Admin" && account.Role != UserRole.Admin)
    {
        HighlightLoginBox(usernameTextBox);
        loginMessageLabel.Text = "Admin access requires an Admin account.";

        MessageBox.Show("Admin access requires an Admin account.", "Access Denied",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    if (selectedAccessType == "Staff" && !accessControlService.CanUseStaffOperations(account))
    {
        HighlightLoginBox(usernameTextBox);
        loginMessageLabel.Text = "This account cannot access staff operations.";

        MessageBox.Show("This account cannot access staff operations.", "Access Denied",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    currentAccount = account;
    ShowDashboardPanel();
}
    private void UpdateShipmentStatus_Click(object? sender, EventArgs e)
    {
        Staff? staff = GetSelectedStaff(staffComboBox);
        Shipment? shipment = GetSelectedShipment(staffShipmentComboBox);

        if (staff == null || shipment == null)
        {
            MessageBox.Show("Please select staff and shipment.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string selectedStatus = statusComboBox.Text;
        string selectedLocation = updateLocationComboBox.Text;

        CaptureConsoleOutput(() =>
        {
            shipmentController.UpdateShipmentStatus(staff, shipment.getShipmentID(), selectedStatus, selectedLocation);
        });

        RefreshShipmentSelectors();

        Shipment? updatedShipment = shipmentController.FindShipmentByID(shipment.getShipmentID());

        if (updatedShipment != null)
        {
            ShowStatusUpdatedResult(staffResultBox, updatedShipment, selectedStatus);
        }
        else
        {
            ShowPlainOutput(staffResultBox, "Status Updated",
                "Shipment " + shipment.getShipmentID() + " has been updated to " + selectedStatus + " at " + selectedLocation + ".");
        }
    }

    private void AddShipment_Click(object? sender, EventArgs e)
{
    Staff? staff = GetSelectedStaff(staffComboBox);

    if (staff == null)
    {
        MessageBox.Show("Please select a staff member.", "Input Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    string shipmentID = newShipmentIDTextBox.Text.Trim();
    string location = newLocationComboBox.Text.Trim();

    if (string.IsNullOrWhiteSpace(shipmentID) || string.IsNullOrWhiteSpace(location))
    {
        MessageBox.Show("Shipment ID and location cannot be blank.", "Input Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    CaptureConsoleOutput(() =>
    {
        shipmentController.AddNewShipment(staff, shipmentID, "In Transit", location, "Not Delivered");
    });

    newShipmentIDTextBox.Clear();
    newLocationComboBox.SelectedIndex = 0;

    RefreshShipmentSelectors();

    Shipment? newShipment = shipmentController.FindShipmentByID(shipmentID);

    if (newShipment != null)
    {
        ClearResultBox(staffResultBox);
        AppendBold(staffResultBox, "Shipment Created" + Environment.NewLine + Environment.NewLine);
        AppendBold(staffResultBox, "Shipment ID: ");
        AppendNormal(staffResultBox, shipmentID + Environment.NewLine);
        AppendBold(staffResultBox, "Shipment Status: ");
        AppendNormal(staffResultBox, "In Transit" + Environment.NewLine);
        AppendBold(staffResultBox, "Starting Location: ");
        AppendNormal(staffResultBox, location + Environment.NewLine);
        AppendBold(staffResultBox, "Delivery Status: ");
        AppendNormal(staffResultBox, "Not Delivered");
    }
    else
    {
        ShowPlainOutput(staffResultBox, "Shipment Created",
            "Shipment " + shipmentID + " has been added successfully.");
    }
}
    private void RefreshRecords_Click(object? sender, EventArgs e)
    {
        RefreshShipmentSelectors();
        SetResultMessage(recordsResultBox, "Shipment records refreshed.");
    }

    private void ViewSelectedRecord_Click(object? sender, EventArgs e)
    {
        Shipment? shipment = GetSelectedShipment(recordsShipmentComboBox);

        if (shipment == null)
        {
            MessageBox.Show("Please select a shipment.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ShowShipmentSummary(recordsResultBox, shipment);
    }

    private void RecordTrackingHistory_Click(object? sender, EventArgs e)
    {
        Shipment? shipment = GetSelectedShipment(recordsShipmentComboBox);

        if (shipment == null)
        {
            MessageBox.Show("Please select a shipment.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ShowTrackingHistory(recordsResultBox, shipment);
    }

    private void RunDepartmentOperation_Click(object? sender, EventArgs e)
    {
        Staff? staff = GetSelectedStaff(operationStaffComboBox);
        Shipment? shipment = GetSelectedShipment(operationShipmentComboBox);

        if (staff == null || shipment == null)
        {
            MessageBox.Show("Please select staff and shipment.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string operationType = GetSelectedOperationType();
        string displayOperation = operationTypeComboBox.Text;
        string department = GetDepartmentFromDisplayText(operationStaffComboBox.Text);
        string resultMessage = "";

        CaptureConsoleOutput(() =>
        {
            resultMessage = shipmentController.RunDepartmentShipmentOperation(staff, shipment.getShipmentID(), operationType);
        });

        ClearResultBox(operationResultBox);

        AppendBold(operationResultBox, "Department Processing Recorded" + Environment.NewLine + Environment.NewLine);
        AppendBold(operationResultBox, "Department: ");
        AppendNormal(operationResultBox, department + Environment.NewLine);
        AppendBold(operationResultBox, "Shipment ID: ");
        AppendNormal(operationResultBox, shipment.getShipmentID() + Environment.NewLine);
        AppendBold(operationResultBox, "Processing Type: ");
        AppendNormal(operationResultBox, displayOperation + Environment.NewLine);
        AppendBold(operationResultBox, "Result: ");
        AppendNormal(operationResultBox, resultMessage + Environment.NewLine + Environment.NewLine);
    }

    private bool IsValidStaffEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        email = email.Trim().ToLower();

        bool hasValidEnding =
            email.EndsWith("@gmail.com") ||
            email.EndsWith("@yahoo.com") ||
            email.EndsWith("@outlook.com") ||
            email.EndsWith("@hotmail.com") ||
            email.EndsWith("@mainfreight.com");

        bool hasOneAtSymbol = email.IndexOf('@') == email.LastIndexOf('@');

        return hasValidEnding && hasOneAtSymbol && email.Length > 10;
    }

    private void ResetLoginInputStyles()
{
    if (usernameTextBox != null)
    {
        usernameTextBox.BackColor = Color.White;
    }

    if (passwordTextBox != null)
    {
        passwordTextBox.BackColor = Color.White;
    }

    if (loginMessageLabel != null)
    {
        loginMessageLabel.ForeColor = Color.Firebrick;
    }
}

private void HighlightLoginBox(TextBox textBox)
{
    textBox.BackColor = Color.FromArgb(255, 225, 225);
}

private bool UsernameExistsInAccountFile(string username)
{
    if (string.IsNullOrWhiteSpace(username))
    {
        return false;
    }

    if (!File.Exists(accountFilePath))
    {
        return false;
    }

    string[] accountLines = File.ReadAllLines(accountFilePath);

    foreach (string line in accountLines)
    {
        string cleanLine = line.Trim();

        if (string.IsNullOrWhiteSpace(cleanLine))
        {
            continue;
        }

        string[] parts = cleanLine.Split('|');

        if (parts.Length > 0 && parts[0].Trim().Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}

    private void AddStaffMember_Click(object? sender, EventArgs e)
    {
        string name = adminStaffNameTextBox.Text.Trim();
        string email = adminStaffEmailTextBox.Text.Trim();
        string department = adminStaffDepartmentComboBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(department))
        {
            MessageBox.Show("Please enter staff name, email, and department.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!IsValidStaffEmail(email))
        {
            MessageBox.Show("Please enter a valid staff email ending in @gmail.com, @yahoo.com, @outlook.com, @hotmail.com, or @mainfreight.com.",
                "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string userID = "User" + (staffMembers.Count + 10);
        string staffID = "Staff" + (staffMembers.Count + 1);

        Staff newStaff = new Staff(userID, name, email, staffID, department);
        staffMembers.Add(newStaff);
        staffDisplayNames.Add(name + " - " + department);

        adminStaffNameTextBox.Clear();
        adminStaffEmailTextBox.Clear();
        adminStaffDepartmentComboBox.SelectedIndex = 0;

        RefreshAdminStaffList();
        RefreshStaffComboBoxes();

        ShowPlainOutput(adminResultBox, "Staff Member Added",
            name + " has been added to the " + department + " department.");
    }

    private void RemoveStaffMember_Click(object? sender, EventArgs e)
    {
        if (adminStaffListBox.SelectedIndex < 0 || adminStaffListBox.SelectedIndex >= staffMembers.Count)
        {
            MessageBox.Show("Please select a staff member to remove.", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string removedStaff = staffDisplayNames[adminStaffListBox.SelectedIndex];

        staffMembers.RemoveAt(adminStaffListBox.SelectedIndex);
        staffDisplayNames.RemoveAt(adminStaffListBox.SelectedIndex);

        RefreshAdminStaffList();
        RefreshStaffComboBoxes();

        ShowPlainOutput(adminResultBox, "Staff Member Removed",
            removedStaff + " has been removed from the staff list for this session.");
    }

    private string GetDepartmentFromDisplayText(string displayText)
    {
        int separatorIndex = displayText.IndexOf(" - ");

        if (separatorIndex < 0)
        {
            return "";
        }

        return displayText.Substring(separatorIndex + 3).Trim();
    }

    private void UpdateAllowedDepartmentProcessing()
    {
        if (operationStaffComboBox == null || operationTypeComboBox == null || operationShipmentComboBox == null)
        {
            return;
        }

        string department = GetDepartmentFromDisplayText(operationStaffComboBox.Text);

        operationTypeComboBox.Items.Clear();

        if (department == "Customer Service")
        {
            operationTypeComboBox.Items.Add("Customer Service Processing");
        }
        else if (department == "Warehouse")
        {
            operationTypeComboBox.Items.Add("Warehouse Processing");
        }
        else if (department == "Returned Goods")
        {
            operationTypeComboBox.Items.Add("Returned Goods Processing");
        }
        else if (department == "Transport" || department == "Operations" || department == "Air and Ocean")
        {
            operationTypeComboBox.Items.Add("Transport Processing");
        }
        else
        {
            operationTypeComboBox.Items.Add("Transport Processing");
        }

        Shipment? selectedShipment = GetSelectedShipment(operationShipmentComboBox);

        if (selectedShipment != null)
        {
            string[] shipmentData = ExtractShipmentData(selectedShipment.getShipmentInfo());
            string shipmentStatus = shipmentData[1];

            if (shipmentStatus == "Returned" && !operationTypeComboBox.Items.Contains("Returned Goods Processing"))
            {
                operationTypeComboBox.Items.Add("Returned Goods Processing");
            }
        }

        if (operationTypeComboBox.Items.Count > 0)
        {
            operationTypeComboBox.SelectedIndex = 0;
        }
    }

    private void UpdateLocationOptionsForStatus()
{
    if (statusComboBox == null || updateLocationComboBox == null)
    {
        return;
    }

    string selectedStatus = statusComboBox.Text;

    updateLocationComboBox.Items.Clear();

    if (selectedStatus == "Delivered")
    {
        updateLocationComboBox.Items.Add("Customer Address");
    }
    else if (selectedStatus == "Out for delivery")
    {
        updateLocationComboBox.Items.Add("Out for Delivery");
        updateLocationComboBox.Items.Add("Customer Address");
    }
    else if (selectedStatus == "Returned")
    {
        updateLocationComboBox.Items.Add("Returned Goods Area");
    }
    else if (selectedStatus == "Delayed")
    {
        updateLocationComboBox.Items.Add("Auckland Depot");
        updateLocationComboBox.Items.Add("Manukau Hub");
        updateLocationComboBox.Items.Add("Hamilton Depot");
        updateLocationComboBox.Items.Add("Wellington Depot");
        updateLocationComboBox.Items.Add("Christchurch Warehouse");
        updateLocationComboBox.Items.Add("In Transit Between Depots");
    }
    else
    {
        updateLocationComboBox.Items.Add("Auckland Depot");
        updateLocationComboBox.Items.Add("Manukau Hub");
        updateLocationComboBox.Items.Add("Hamilton Depot");
        updateLocationComboBox.Items.Add("Wellington Depot");
        updateLocationComboBox.Items.Add("Christchurch Warehouse");
        updateLocationComboBox.Items.Add("In Transit Between Depots");
    }

    if (updateLocationComboBox.Items.Count > 0)
    {
        updateLocationComboBox.SelectedIndex = 0;
    }
}

    private string[] GetLocationOptions()
    {
        return new string[]
        {
            "Auckland Depot",
            "Manukau Hub",
            "Hamilton Depot",
            "Wellington Depot",
            "Christchurch Warehouse",
            "In Transit Between Depots",
            "Out for Delivery",
            "Customer Address",
            "Returned Goods Area"
        };
    }

    private string[] GetDepotLocationOptions()
    {
        return new string []
        {
            "Auckland Depot",
            "Manukau Hub",
            "Hamilton Depot",
            "Wellington Depot",
            "Christchurch Warehouse"

        };
        
    }
    private void ShowStatusUpdatedResult(RichTextBox box, Shipment shipment, string selectedStatus)
    {
        string[] updatedData = ExtractShipmentData(shipment.getShipmentInfo());

        ClearResultBox(box);
        AppendBold(box, "Status Updated" + Environment.NewLine + Environment.NewLine);
        AppendBold(box, "Shipment ID: ");
        AppendNormal(box, updatedData[0] + Environment.NewLine);
        AppendBold(box, "New Status: ");
        AppendNormal(box, selectedStatus + Environment.NewLine);
        AppendBold(box, "Current Location: ");
        AppendNormal(box, updatedData[2] + Environment.NewLine);
        AppendBold(box, "Delivery Status: ");
        AppendNormal(box, updatedData[3] + Environment.NewLine + Environment.NewLine);
        AppendBold(box, "Tracking Update: ");
        AppendNormal(box, "This status and location update has been added to the shipment tracking history.");
    }

    private string GetSelectedOperationType()
    {
        string selectedText = operationTypeComboBox.Text.Trim().ToLower();

        if (selectedText.Contains("customer"))
        {
            return "customerservice";
        }

        if (selectedText.Contains("warehouse"))
        {
            return "warehouse";
        }

        if (selectedText.Contains("return"))
        {
            return "return";
        }

        return "transport";
    }

    private void RefreshShipmentSelectors()
    {
        ComboBox[] selectors =
        {
            publicShipmentComboBox,
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

            string previousSelection = selector.SelectedItem?.ToString() ?? "";

            selector.Items.Clear();

            foreach (Shipment shipment in shipmentController.GetAllShipments())
            {
                selector.Items.Add(shipment.getShipmentID());
            }

            if (selector.Items.Count > 0)
            {
                int existingIndex = selector.Items.IndexOf(previousSelection);
                selector.SelectedIndex = existingIndex >= 0 ? existingIndex : 0;
            }
        }
    }

    private void RefreshStaffComboBoxes()
    {
        LoadStaffOptions(staffComboBox);
        LoadStaffOptions(operationStaffComboBox);
        UpdateAllowedDepartmentProcessing();
    }

    private void RefreshAdminStaffList()
    {
        if (adminStaffListBox == null)
        {
            return;
        }

        adminStaffListBox.Items.Clear();

        foreach (string staffDisplayName in staffDisplayNames)
        {
            adminStaffListBox.Items.Add(staffDisplayName);
        }
    }

    private void LoadStaffOptions(ComboBox comboBox)
    {
        if (comboBox == null)
        {
            return;
        }

        string previousSelection = comboBox.SelectedItem?.ToString() ?? "";

        comboBox.Items.Clear();

        foreach (string staffDisplayName in staffDisplayNames)
        {
            comboBox.Items.Add(staffDisplayName);
        }

        if (comboBox.Items.Count > 0)
        {
            int existingIndex = comboBox.Items.IndexOf(previousSelection);
            comboBox.SelectedIndex = existingIndex >= 0 ? existingIndex : 0;
        }
    }

    private Staff? GetSelectedStaff(ComboBox comboBox)
    {
        if (comboBox.SelectedIndex < 0 || comboBox.SelectedIndex >= staffMembers.Count)
        {
            return null;
        }

        return staffMembers[comboBox.SelectedIndex];
    }

    private Shipment? GetSelectedShipment(ComboBox comboBox)
    {
        if (comboBox.SelectedItem == null)
        {
            return null;
        }

        return shipmentController.FindShipmentByID(comboBox.SelectedItem.ToString() ?? "");
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

    private void ShowShipmentSummary(RichTextBox box, Shipment shipment)
    {
        string[] data = ExtractShipmentData(shipment.getShipmentInfo());

        ClearResultBox(box);

        AppendBold(box, "Shipment ID: ");
        AppendNormal(box, data[0] + Environment.NewLine);

        AppendBold(box, "Shipment Status: ");
        AppendNormal(box, data[1] + Environment.NewLine);

        AppendBold(box, "Current Location: ");
        AppendNormal(box, data[2] + Environment.NewLine);

        AppendBold(box, "Delivery Status: ");
        AppendNormal(box, data[3]);
    }

    private void ShowTrackingHistory(RichTextBox box, Shipment shipment)
    {
        ClearResultBox(box);

        AppendBold(box, "Tracking History" + Environment.NewLine + Environment.NewLine);

        string historyOutput = CaptureConsoleOutput(() =>
        {
            shipment.viewTrackingHistory();
        });

        string[] historyLines = historyOutput.Split(
            new[] { Environment.NewLine, "\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        bool hasVisibleHistory = false;

        foreach (string line in historyLines)
        {
            string cleanLine = line.Trim();

            if (cleanLine.StartsWith("Tracking updates for shipment"))
            {
                continue;
            }

            if (cleanLine.StartsWith("Time:"))
            {
                AppendBold(box, "Time: ");
                AppendNormal(box, cleanLine.Replace("Time:", "").Trim() + Environment.NewLine);
                hasVisibleHistory = true;
            }
            else if (cleanLine.StartsWith("Message:"))
            {
                AppendBold(box, "Message: ");
                AppendNormal(box, cleanLine.Replace("Message:", "").Trim() + Environment.NewLine + Environment.NewLine);
                hasVisibleHistory = true;
            }
            else if (!string.IsNullOrWhiteSpace(cleanLine))
            {
                AppendNormal(box, cleanLine + Environment.NewLine);
                hasVisibleHistory = true;
            }
        }

        if (!hasVisibleHistory)
        {
            AppendNormal(box, "No tracking updates are currently recorded for this shipment.");
        }
    }

    private void ShowPlainOutput(RichTextBox box, string heading, string message)
    {
        ClearResultBox(box);

        AppendBold(box, heading + Environment.NewLine + Environment.NewLine);
        AppendNormal(box, message);
    }

    private void SetResultMessage(RichTextBox box, string message)
    {
        ClearResultBox(box);
        AppendNormal(box, message);
    }

    private void ClearResultBox(RichTextBox box)
    {
        box.Clear();
        box.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
        box.SelectionColor = Color.FromArgb(35, 45, 55);
    }

    private void AppendBold(RichTextBox box, string text)
    {
        box.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
        box.SelectionColor = textDark;
        box.AppendText(text);
    }

    private void AppendNormal(RichTextBox box, string text)
    {
        box.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
        box.SelectionColor = Color.FromArgb(35, 45, 55);
        box.AppendText(text);
    }

    private void RegisterShipmentStatusListeners(Shipment shipment)
    {
        shipment.AttachStatusListener(new TrackingUpdateRecorder());
        shipment.AttachStatusListener(new CustomerStatusNotifier());
        shipment.AttachStatusListener(new StaffStatusNotifier());
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

    private void SaveShipmentsToFile(List<Shipment> shipmentList, string shipmentFilePath)
    {
        List<string> lines = new List<string>();

        foreach (Shipment shipment in shipmentList)
        {
            string shipmentInfo = shipment.getShipmentInfo();
            string[] shipmentParts = ExtractShipmentData(shipmentInfo);

            lines.Add(shipmentParts[0] + "|" + shipmentParts[1] + "|" + shipmentParts[2] + "|" + shipmentParts[3]);
        }

        File.WriteAllLines(shipmentFilePath, lines);
    }

    private string[] ExtractShipmentData(string shipmentInfo)
    {
        string[] lines = shipmentInfo.Split('\n');

        string shipmentID = lines.Length > 0 ? lines[0].Replace("Shipment ID:", "").Trim() : "";
        string shipmentStatus = lines.Length > 1 ? lines[1].Replace("Shipment Status:", "").Trim() : "";
        string currentLocation = lines.Length > 2 ? lines[2].Replace("Current Location:", "").Trim() : "";
        string deliveryStatus = lines.Length > 3 ? lines[3].Replace("Delivery Status:", "").Trim() : "";

        return new string[] { shipmentID, shipmentStatus, currentLocation, deliveryStatus };
    }

    private TabPage CreateTabPage(string title)
    {
        TabPage tab = new TabPage(title);
        tab.BackColor = softBlue;
        tab.Padding = new Padding(8);
        return tab;
    }

    private Label AddTitle(Control parent, string text, int x, int y, int width)
    {
        Label label = new Label();
        label.Text = text;
        label.Font = new Font("Segoe UI", 22, FontStyle.Bold);
        label.ForeColor = textDark;
        label.Location = new Point(x, y);
        label.Size = new Size(width, 58);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.UseCompatibleTextRendering = true;
        parent.Controls.Add(label);
        return label;
    }

    private void AddSectionHeading(Control parent, string text, int x, int y)
    {
        Label label = new Label();
        label.Text = text;
        label.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        label.ForeColor = textDark;
        label.Location = new Point(x, y);
        label.Size = new Size(700, 44);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.UseCompatibleTextRendering = false;
        parent.Controls.Add(label);
        label.BringToFront();
    }

    private Label AddSectionHeadingLabel(Control parent, string text, int x, int y, int width)
    {
        Label label = new Label();
        label.Text = text;
        label.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        label.ForeColor = textDark;
        label.Location = new Point(x, y);
        label.Size = new Size(width, 44);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.UseCompatibleTextRendering = false;
        parent.Controls.Add(label);
        return label;
    }

    private Label AddPlainText(Control parent, string text, int x, int y, int width)
    {
        Label label = new Label();
        label.Text = text;
        label.Location = new Point(x, y);
        label.Size = new Size(width, 56);
        label.Font = new Font("Segoe UI", 10);
        label.ForeColor = Color.DimGray;
        label.TextAlign = ContentAlignment.TopLeft;
        label.UseCompatibleTextRendering = true;
        parent.Controls.Add(label);
        return label;
    }

    private RichTextBox AddResultBox(Control parent, int x, int y, int width, int height)
    {
        RichTextBox box = new RichTextBox();
        box.Location = new Point(x, y);
        box.Size = new Size(width, height);
        box.Font = new Font("Segoe UI", 10);
        box.ForeColor = Color.FromArgb(35, 45, 55);
        box.BackColor = palePanel;
        box.BorderStyle = BorderStyle.None;
        box.ReadOnly = true;
        box.TabStop = false;
        box.ScrollBars = RichTextBoxScrollBars.Vertical;
        box.Cursor = Cursors.Default;
        box.WordWrap = true;
        parent.Controls.Add(box);
        return box;
    }

    private Label AddLabel(Control parent, string text, int x, int y)
    {
        Label label = new Label();
        label.Text = text;
        label.Location = new Point(x, y);
        label.Size = new Size(140, 30);
        label.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        label.ForeColor = Color.FromArgb(35, 45, 55);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.UseCompatibleTextRendering = false;
        parent.Controls.Add(label);
        return label;
    }

    private TextBox AddTextBox(Control parent, int x, int y, int width)
    {
        TextBox textBox = new TextBox();
        textBox.Location = new Point(x, y);
        textBox.Size = new Size(width, 34);
        textBox.Font = new Font("Segoe UI", 10);
        textBox.BorderStyle = BorderStyle.FixedSingle;
        parent.Controls.Add(textBox);
        return textBox;
    }

    private ComboBox AddComboBox(Control parent, int x, int y, int width)
    {
        ComboBox comboBox = new ComboBox();
        comboBox.Location = new Point(x, y);
        comboBox.Size = new Size(width, 32);
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Font = new Font("Segoe UI", 10);
        comboBox.FlatStyle = FlatStyle.Standard;
        parent.Controls.Add(comboBox);
        comboBox.BringToFront();
        return comboBox;
    }

    private Button AddPrimaryButton(Control parent, string text, int x, int y, int width)
    {
        Button button = new Button();
        button.Text = text;
        button.Location = new Point(x, y);
        button.Size = new Size(width, 36);
        button.BackColor = mainfreightBlue;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        parent.Controls.Add(button);
        return button;
    }

    private Button AddSecondaryButton(Control parent, string text, int x, int y, int width)
    {
        Button button = new Button();
        button.Text = text;
        button.Location = new Point(x, y);
        button.Size = new Size(width, 36);
        button.BackColor = Color.White;
        button.ForeColor = mainfreightBlue;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = mainfreightBlue;
        button.FlatAppearance.BorderSize = 1;
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        parent.Controls.Add(button);
        return button;
    }
}