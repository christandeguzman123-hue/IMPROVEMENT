using System.Data;
using Rental_Business_System.Data;

namespace Rental_Business_System;

internal sealed class ClientPortalForm : Form
{
    private readonly UserSession _session;
    private readonly RentalRepository _repository = new(SqliteDatabase.ConnectionString);

    private readonly Label _welcomeLabel = new();
    private readonly DataGridView _inventoryGrid = CreateGrid();
    private readonly DataGridView _myRentalsGrid = CreateGrid();
    private readonly ComboBox _itemCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
    private readonly NumericUpDown _quantity = new() { Minimum = 1, Maximum = 1000, Value = 1, Width = 140 };
    private readonly DateTimePicker _start = new() { Format = DateTimePickerFormat.Short, Width = 140 };
    private readonly DateTimePicker _end = new() { Format = DateTimePickerFormat.Short, Width = 140 };
    private Label label1;
    private readonly CheckBox _openEnded = new() { Text = "Open-ended rental" };

    public ClientPortalForm(UserSession session)
    {
        _session = session;

        Text = "Client Portal";
        Width = 1020;
        Height = 680;
        StartPosition = FormStartPosition.CenterScreen;

        _welcomeLabel.Dock = DockStyle.Top;
        _welcomeLabel.Height = 40;
        _welcomeLabel.TextAlign = ContentAlignment.MiddleLeft;
        _welcomeLabel.Padding = new Padding(10, 0, 0, 0);
        _welcomeLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);

        TabControl tabs = new() { Dock = DockStyle.Fill };
        tabs.TabPages.Add(CreateInventoryTab());
        tabs.TabPages.Add(CreateMyRentalsTab());

        Controls.Add(tabs);
        Controls.Add(_welcomeLabel);

        Load += (_, _) => RefreshAll();
    }

    private TabPage CreateInventoryTab()
    {
        TabPage page = new("Available Inventory");

        FlowLayoutPanel panel = CreateTopPanel();
        _openEnded.CheckedChanged += (_, _) => _end.Enabled = !_openEnded.Checked;

        panel.Controls.Add(CreateLabeledInput("Item", _itemCombo));
        panel.Controls.Add(CreateLabeledInput("Quantity", _quantity));
        panel.Controls.Add(CreateLabeledInput("Start", _start));
        panel.Controls.Add(CreateLabeledInput("End", _end));
        panel.Controls.Add(_openEnded);
        panel.Controls.Add(CreateButton("Create Rental Request", (_, _) => CreateRental()));

        page.Controls.Add(_inventoryGrid);
        page.Controls.Add(panel);
        return page;
    }

    private TabPage CreateMyRentalsTab()
    {
        TabPage page = new("My Rentals");

        FlowLayoutPanel panel = CreateTopPanel();
        panel.Controls.Add(CreateButton("Refresh", (_, _) => RefreshMyRentals()));

        page.Controls.Add(_myRentalsGrid);
        page.Controls.Add(panel);
        return page;
    }

    private void RefreshAll()
    {
        _welcomeLabel.Text = $"Welcome, {_session.Username} (Client)";

        DataTable available = _repository.GetAvailableInventory();
        _inventoryGrid.DataSource = available;

        _itemCombo.DataSource = available;
        _itemCombo.DisplayMember = "ItemName";
        _itemCombo.ValueMember = "Id";

        RefreshMyRentals();
    }

    private void RefreshMyRentals()
    {
        _myRentalsGrid.DataSource = _repository.GetRentalsByUser(_session.UserId);
    }

    private void CreateRental()
    {
        if (_itemCombo.SelectedValue is null)
        {
            MessageBox.Show("No available item selected.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DateTime? endDate = _openEnded.Checked ? null : _end.Value.Date;
        if (endDate.HasValue && endDate.Value.Date < _start.Value.Date)
        {
            MessageBox.Show("End date cannot be earlier than start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            long itemId = Convert.ToInt64(_itemCombo.SelectedValue);
            _repository.CreateRentalForUser(_session.UserId, _session.Username, itemId, (int)_quantity.Value, _start.Value.Date, endDate);
            MessageBox.Show("Rental request created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
    }

    private static FlowLayoutPanel CreateTopPanel()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 104,
            Padding = new Padding(8),
            AutoScroll = true,
            WrapContents = true
        };
    }

    private static Control CreateLabeledInput(string labelText, Control input)
    {
        Panel panel = new() { Width = 230, Height = 70, Margin = new Padding(6) };
        Label label = new() { Text = labelText, Dock = DockStyle.Top, Height = 18 };
        input.Dock = DockStyle.Bottom;
        input.Height = 28;
        panel.Controls.Add(input);
        panel.Controls.Add(label);
        return panel;
    }

    private void InitializeComponent()
    {
        label1 = new Label();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(106, 79);
        label1.Name = "label1";
        label1.Size = new Size(0, 15);
        label1.TabIndex = 0;
        // 
        // ClientPortalForm
        // 
        ClientSize = new Size(302, 272);
        Controls.Add(label1);
        Name = "ClientPortalForm";
        Load += ClientPortalForm_Load;
        ResumeLayout(false);
        PerformLayout();

    }

    private static Button CreateButton(string text, EventHandler onClick)
    {
        Button button = new()
        {
            Text = text,
            Width = 190,
            Height = 36,
            Margin = new Padding(6, 24, 6, 6)
        };
        button.Click += onClick;
        return button;
    }

    private void ClientPortalForm_Load(object sender, EventArgs e)
    {

    }
}
