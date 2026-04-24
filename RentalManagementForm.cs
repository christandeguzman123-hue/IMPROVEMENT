using System.Data;
using Rental_Business_System.Data;

namespace Rental_Business_System;

internal sealed class RentalManagementForm : Form
{
    private readonly RentalRepository _repository = new(SqliteDatabase.ConnectionString);
    private readonly UserSession _adminSession;

    private readonly DataGridView _customersGrid = CreateGrid();
    private readonly DataGridView _inventoryGrid = CreateGrid();
    private readonly DataGridView _rentalsGrid = CreateGrid();
    private readonly DataGridView _reportsGrid = CreateGrid();

    private readonly TextBox _customerName = new();
    private readonly TextBox _customerPhone = new();
    private readonly TextBox _customerEmail = new();
    private readonly TextBox _customerAddress = new();

    private readonly TextBox _itemName = new();
    private readonly TextBox _itemCategory = new();
    private readonly NumericUpDown _itemRate = new() { DecimalPlaces = 2, Maximum = 1000000, Width = 140 };
    private readonly NumericUpDown _itemQuantity = new() { Maximum = 1000000, Width = 140 };

    private readonly ComboBox _rentalCustomer = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 230 };
    private readonly ComboBox _rentalItem = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 230 };
    private readonly NumericUpDown _rentalQuantity = new() { Minimum = 1, Maximum = 1000000, Value = 1, Width = 140 };
    private readonly DateTimePicker _rentalStart = new() { Format = DateTimePickerFormat.Short, Width = 140 };
    private readonly DateTimePicker _rentalEnd = new() { Format = DateTimePickerFormat.Short, Width = 140 };
    private readonly CheckBox _openEndedRental = new() { Text = "Open-ended rental" };

    internal RentalManagementForm(UserSession adminSession)
    {
        _adminSession = adminSession;

        if (!string.Equals(_adminSession.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only admin users can open the management system.");
        }

        Text = "Rental Business System - SQLite";
        Width = 1180;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;

        TabControl tabControl = new() { Dock = DockStyle.Fill };
        tabControl.TabPages.Add(CreateCustomersPage());
        tabControl.TabPages.Add(CreateInventoryPage());
        tabControl.TabPages.Add(CreateRentalsPage());
        tabControl.TabPages.Add(CreateReportsPage());

        Controls.Add(tabControl);
        Load += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        RefreshAll();
    }

    private TabPage CreateCustomersPage()
    {
        TabPage page = new("Customers");

        FlowLayoutPanel editor = CreateEditorPanel();
        editor.Controls.AddRange(new Control[]
        {
            CreateLabeledInput("Full Name", _customerName),
            CreateLabeledInput("Phone", _customerPhone),
            CreateLabeledInput("Email", _customerEmail),
            CreateLabeledInput("Address", _customerAddress),
            CreateButton("Add", (_, _) => AddCustomer()),
            CreateButton("Update Selected", (_, _) => UpdateCustomer()),
            CreateButton("Delete Selected", (_, _) => DeleteCustomer())
        });

        _customersGrid.SelectionChanged += (_, _) => FillCustomerEditorFromSelection();

        page.Controls.Add(_customersGrid);
        page.Controls.Add(editor);
        return page;
    }

    private TabPage CreateInventoryPage()
    {
        TabPage page = new("Inventory");

        FlowLayoutPanel editor = CreateEditorPanel();
        editor.Controls.AddRange(new Control[]
        {
            CreateLabeledInput("Item Name", _itemName),
            CreateLabeledInput("Category", _itemCategory),
            CreateLabeledInput("Daily Rate", _itemRate),
            CreateLabeledInput("Quantity Available", _itemQuantity),
            CreateButton("Add", (_, _) => AddInventoryItem()),
            CreateButton("Update Selected", (_, _) => UpdateInventoryItem()),
            CreateButton("Delete Selected", (_, _) => DeleteInventoryItem())
        });

        _inventoryGrid.SelectionChanged += (_, _) => FillInventoryEditorFromSelection();

        page.Controls.Add(_inventoryGrid);
        page.Controls.Add(editor);
        return page;
    }

    private TabPage CreateRentalsPage()
    {
        TabPage page = new("Rentals");

        FlowLayoutPanel editor = CreateEditorPanel();
        _openEndedRental.CheckedChanged += (_, _) => _rentalEnd.Enabled = !_openEndedRental.Checked;

        editor.Controls.AddRange(new Control[]
        {
            CreateLabeledInput("Customer", _rentalCustomer),
            CreateLabeledInput("Item", _rentalItem),
            CreateLabeledInput("Quantity", _rentalQuantity),
            CreateLabeledInput("Start Date", _rentalStart),
            CreateLabeledInput("End Date", _rentalEnd),
            _openEndedRental,
            CreateButton("Create Rental", (_, _) => CreateRental()),
            CreateButton("Approve Selected", (_, _) => ApproveSelectedRental()),
            CreateButton("Set Pending", (_, _) => SetSelectedRentalPending()),
            CreateButton("Mark Selected Returned", (_, _) => MarkRentalReturned())
        });

        page.Controls.Add(_rentalsGrid);
        page.Controls.Add(editor);
        return page;
    }

    private TabPage CreateReportsPage()
    {
        TabPage page = new("Reports");

        FlowLayoutPanel panel = CreateEditorPanel();
        panel.Controls.Add(CreateButton("Refresh Metrics", (_, _) => RefreshReports()));

        page.Controls.Add(_reportsGrid);
        page.Controls.Add(panel);
        return page;
    }

    private void RefreshAll()
    {
        RefreshCustomers();
        RefreshInventory();
        RefreshRentals();
        RefreshReports();
    }

    private void RefreshCustomers()
    {
        _customersGrid.DataSource = _repository.GetCustomers();

        DataTable table = _repository.GetCustomers();
        _rentalCustomer.DataSource = table;
        _rentalCustomer.DisplayMember = "FullName";
        _rentalCustomer.ValueMember = "Id";
    }

    private void RefreshInventory()
    {
        _inventoryGrid.DataSource = _repository.GetInventory();

        DataTable table = _repository.GetInventory();
        _rentalItem.DataSource = table;
        _rentalItem.DisplayMember = "ItemName";
        _rentalItem.ValueMember = "Id";
    }

    private void RefreshRentals()
    {
        _rentalsGrid.DataSource = _repository.GetRentals();
    }

    private void RefreshReports()
    {
        _reportsGrid.DataSource = _repository.GetSummary();
    }

    private void AddCustomer()
    {
        if (string.IsNullOrWhiteSpace(_customerName.Text))
        {
            MessageBox.Show("Customer name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.AddCustomer(_customerName.Text.Trim(), _customerPhone.Text.Trim(), _customerEmail.Text.Trim(), _customerAddress.Text.Trim());
            ClearCustomerEditor();
            RefreshCustomers();
        });
    }

    private void UpdateCustomer()
    {
        if (!TryGetSelectedId(_customersGrid, out long id))
        {
            MessageBox.Show("Select a customer row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_customerName.Text))
        {
            MessageBox.Show("Customer name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.UpdateCustomer(id, _customerName.Text.Trim(), _customerPhone.Text.Trim(), _customerEmail.Text.Trim(), _customerAddress.Text.Trim());
            RefreshCustomers();
        });
    }

    private void DeleteCustomer()
    {
        if (!TryGetSelectedId(_customersGrid, out long id))
        {
            MessageBox.Show("Select a customer row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Delete selected customer?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        RunSafe(() =>
        {
            _repository.DeleteCustomer(id);
            ClearCustomerEditor();
            RefreshCustomers();
        });
    }

    private void AddInventoryItem()
    {
        if (string.IsNullOrWhiteSpace(_itemName.Text))
        {
            MessageBox.Show("Item name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.AddInventoryItem(_itemName.Text.Trim(), _itemCategory.Text.Trim(), _itemRate.Value, (int)_itemQuantity.Value);
            ClearInventoryEditor();
            RefreshInventory();
        });
    }

    private void UpdateInventoryItem()
    {
        if (!TryGetSelectedId(_inventoryGrid, out long id))
        {
            MessageBox.Show("Select an inventory row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_itemName.Text))
        {
            MessageBox.Show("Item name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.UpdateInventoryItem(id, _itemName.Text.Trim(), _itemCategory.Text.Trim(), _itemRate.Value, (int)_itemQuantity.Value);
            RefreshInventory();
        });
    }

    private void DeleteInventoryItem()
    {
        if (!TryGetSelectedId(_inventoryGrid, out long id))
        {
            MessageBox.Show("Select an inventory row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Delete selected inventory item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        RunSafe(() =>
        {
            _repository.DeleteInventoryItem(id);
            ClearInventoryEditor();
            RefreshInventory();
        });
    }

    private void CreateRental()
    {
        if (_rentalCustomer.SelectedValue is not long customerId && _rentalCustomer.SelectedValue is not int)
        {
            MessageBox.Show("Add a customer first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_rentalItem.SelectedValue is not long itemId && _rentalItem.SelectedValue is not int)
        {
            MessageBox.Show("Add an inventory item first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        long customer = Convert.ToInt64(_rentalCustomer.SelectedValue);
        long item = Convert.ToInt64(_rentalItem.SelectedValue);
        DateTime? endDate = _openEndedRental.Checked ? null : _rentalEnd.Value.Date;

        if (endDate.HasValue && endDate.Value.Date < _rentalStart.Value.Date)
        {
            MessageBox.Show("End date cannot be earlier than start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.CreateRental(customer, item, (int)_rentalQuantity.Value, _rentalStart.Value.Date, endDate);
            RefreshAll();
        });
    }

    private void MarkRentalReturned()
    {
        if (!TryGetSelectedId(_rentalsGrid, out long id))
        {
            MessageBox.Show("Select a rental row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.MarkRentalReturned(id);
            RefreshAll();
        });
    }

    private void ApproveSelectedRental()
    {
        if (!TryGetSelectedId(_rentalsGrid, out long id))
        {
            MessageBox.Show("Select a rental row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.UpdateRentalStatus(id, "Approved");
            RefreshRentals();
            RefreshReports();
        });
    }

    private void SetSelectedRentalPending()
    {
        if (!TryGetSelectedId(_rentalsGrid, out long id))
        {
            MessageBox.Show("Select a rental row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunSafe(() =>
        {
            _repository.UpdateRentalStatus(id, "Pending");
            RefreshRentals();
            RefreshReports();
        });
    }

    private void FillCustomerEditorFromSelection()
    {
        if (_customersGrid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            return;
        }

        _customerName.Text = row["FullName"]?.ToString() ?? string.Empty;
        _customerPhone.Text = row["Phone"]?.ToString() ?? string.Empty;
        _customerEmail.Text = row["Email"]?.ToString() ?? string.Empty;
        _customerAddress.Text = row["Address"]?.ToString() ?? string.Empty;
    }

    private void FillInventoryEditorFromSelection()
    {
        if (_inventoryGrid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            return;
        }

        _itemName.Text = row["ItemName"]?.ToString() ?? string.Empty;
        _itemCategory.Text = row["Category"]?.ToString() ?? string.Empty;

        if (decimal.TryParse(row["DailyRate"]?.ToString(), out decimal rate))
        {
            _itemRate.Value = Math.Min(_itemRate.Maximum, Math.Max(_itemRate.Minimum, rate));
        }

        if (int.TryParse(row["QuantityAvailable"]?.ToString(), out int qty))
        {
            _itemQuantity.Value = Math.Min(_itemQuantity.Maximum, Math.Max(_itemQuantity.Minimum, qty));
        }
    }

    private void ClearCustomerEditor()
    {
        _customerName.Clear();
        _customerPhone.Clear();
        _customerEmail.Clear();
        _customerAddress.Clear();
    }

    private void ClearInventoryEditor()
    {
        _itemName.Clear();
        _itemCategory.Clear();
        _itemRate.Value = 0;
        _itemQuantity.Value = 0;
    }

    private static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            MultiSelect = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            BackgroundColor = Color.White
        };
    }

    private static FlowLayoutPanel CreateEditorPanel()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 120,
            Padding = new Padding(8),
            AutoScroll = true,
            WrapContents = true
        };
    }

    private static Control CreateLabeledInput(string labelText, Control input)
    {
        Panel panel = new() { Width = 235, Height = 72, Margin = new Padding(6) };
        Label label = new() { Text = labelText, Dock = DockStyle.Top, Height = 18 };
        input.Dock = DockStyle.Bottom;
        input.Height = 28;

        if (input is TextBox textBox)
        {
            textBox.Width = panel.Width - 4;
        }

        panel.Controls.Add(input);
        panel.Controls.Add(label);
        return panel;
    }

    private static Button CreateButton(string text, EventHandler onClick)
    {
        Button button = new()
        {
            Text = text,
            Width = 160,
            Height = 36,
            Margin = new Padding(6, 24, 6, 6)
        };

        button.Click += onClick;
        return button;
    }

    private static bool TryGetSelectedId(DataGridView grid, out long id)
    {
        id = 0;
        if (grid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            return false;
        }

        return long.TryParse(row["Id"]?.ToString(), out id);
    }

    private void RunSafe(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
