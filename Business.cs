using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rental_Business_System.Data;

namespace Rental_Business_System
{
    public partial class Business : Form
    {
        private readonly RentalRepository _repository = new(SqliteDatabase.ConnectionString);
        private readonly List<ItemRow> _itemRows = new();
        private long? _currentUserId;
        private string _currentUsername = "Guest";
        private bool _myRentalsOnly;

        public Business()
        {
            InitializeComponent();
            InitializeRuntimeBindings();
        }

        internal Business(long userId, string username) : this()
        {
            _currentUserId = userId;
            _currentUsername = string.IsNullOrWhiteSpace(username) ? "Client" : username.Trim();
        }

        private void Business_Load(object sender, EventArgs e)
        {
            textBox1.PlaceholderText = "Search equipment...";
            RefreshDashboard();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RentByButton(button1);
        }

        private void InitializeRuntimeBindings()
        {
            textBox1.TextChanged += (_, _) => ApplySearchFilter();

            _itemRows.Add(new ItemRow(panel7, label19, label20, label21, button1));
            _itemRows.Add(new ItemRow(panel8, label23, label32, label41, button2));
            _itemRows.Add(new ItemRow(panel9, label24, label33, label42, button3));
            _itemRows.Add(new ItemRow(panel10, label25, label34, label43, button4));
            _itemRows.Add(new ItemRow(panel11, label26, label35, label44, button7));
            _itemRows.Add(new ItemRow(panel12, label27, label36, label45, button6));
            _itemRows.Add(new ItemRow(panel13, label28, label37, label46, button5));
            _itemRows.Add(new ItemRow(panel14, label29, label38, label47, button9));
            _itemRows.Add(new ItemRow(panel15, label30, label39, label48, button8));
            _itemRows.Add(new ItemRow(panel16, label31, label40, label49, button10));

            foreach (ItemRow row in _itemRows)
            {
                row.RentButton.Click += RentButton_Click;
            }

            ShowAllEquipment();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ShowAllEquipment();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ShowAllEquipment();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ShowMyRentals();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            HandleLogout();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            PromptClientLoginIfNeeded();
        }

        private void RefreshDashboard()
        {
            try
            {
                DataTable inventory = _repository.GetInventory();
                DataTable myRentals = _currentUserId.HasValue ? _repository.GetRentalsByUser(_currentUserId.Value) : new DataTable();

                var inventoryByName = inventory.AsEnumerable()
                    .ToDictionary(
                        row => row.Field<string>("ItemName") ?? string.Empty,
                        row => row,
                        StringComparer.OrdinalIgnoreCase);

                var latestRentalByItem = myRentals.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Item") ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.First(),
                        StringComparer.OrdinalIgnoreCase);

                foreach (ItemRow row in _itemRows)
                {
                    string itemName = row.ItemLabel.Text.Trim();
                    bool found = inventoryByName.TryGetValue(itemName, out DataRow? itemData);
                    int quantity = found ? Convert.ToInt32(itemData!["QuantityAvailable"]) : 0;

                    row.ItemId = found ? Convert.ToInt64(itemData!["Id"]) : null;
                    row.RentButton.Enabled = found && quantity > 0;
                    row.RentButton.Text = row.RentButton.Enabled ? "Rent" : "Out";

                    if (latestRentalByItem.TryGetValue(itemName, out DataRow? rentalData))
                    {
                        row.StartDateLabel.Text = FormatDate(rentalData["StartDate"]?.ToString());
                        row.ReturnDateLabel.Text = FormatDate(rentalData["EndDate"]?.ToString());

                        bool active = string.Equals(rentalData["Status"]?.ToString(), "Active", StringComparison.OrdinalIgnoreCase);
                        row.HasClientRental = true;
                        if (active)
                        {
                            row.RentButton.Enabled = false;
                            row.RentButton.Text = "Rented";
                        }
                    }
                    else
                    {
                        row.StartDateLabel.Text = "-";
                        row.ReturnDateLabel.Text = "-";
                        row.HasClientRental = false;
                    }
                }

                (int totalCustomers, int activeRentals, int overdueRentals) = _repository.GetDashboardCounts(_currentUserId);

                if (_currentUserId.HasValue)
                {
                    // Client view: show only client-relevant metrics.
                    label7.Text = "My Account";
                    label9.Text = "My Active Rentals";
                    label11.Text = "My Overdue Rentals";

                    label8.Text = "1";
                    label10.Text = activeRentals.ToString();
                    label12.Text = overdueRentals.ToString();
                }
                else
                {
                    // Guest view: show global overview.
                    label7.Text = "Total Customer";
                    label9.Text = "Active Rentals";
                    label11.Text = "OverDue Rentals";

                    label8.Text = totalCustomers.ToString();
                    label10.Text = activeRentals.ToString();
                    label12.Text = overdueRentals.ToString();
                }

                label5.Text = _currentUserId.HasValue
                    ? $"Client Equipment - {_currentUsername}"
                    : "Client Equipment";

                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RentButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }

            RentByButton(button);
        }

        private void RentByButton(Button button)
        {
            ItemRow? row = _itemRows.FirstOrDefault(r => r.RentButton == button);
            if (row is null)
            {
                return;
            }

            if (!_currentUserId.HasValue)
            {
                MessageBox.Show("Please login as a client before renting equipment.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SystemRental loginForm = new SystemRental();
                loginForm.Show();
                return;
            }

            if (!row.ItemId.HasValue)
            {
                MessageBox.Show("This equipment item is not yet in the database.", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Today.AddDays(3);
                _repository.CreateRentalForUser(_currentUserId.Value, _currentUsername, row.ItemId.Value, 1, startDate, endDate);
                MessageBox.Show($"Rental created for {row.ItemLabel.Text}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Rental Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleLogout()
        {
            if (_currentUserId.HasValue)
            {
                MessageBox.Show("You are now logged out.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Owner is Form owner)
            {
                owner.Show();
            }
            Close();
        }

        private void ApplySearchFilter()
        {
            string query = textBox1.Text.Trim();
            foreach (ItemRow row in _itemRows)
            {
                bool matchesQuery = string.IsNullOrWhiteSpace(query) ||
                    row.ItemLabel.Text.Contains(query, StringComparison.OrdinalIgnoreCase);
                bool matchesMyRentalsFilter = !_myRentalsOnly || row.HasClientRental;
                row.Panel.Visible = matchesQuery && matchesMyRentalsFilter;
            }
        }

        private void ShowAllEquipment()
        {
            _myRentalsOnly = false;
            label5.Text = _currentUserId.HasValue
                ? $"Client Equipment - {_currentUsername}"
                : "Client Equipment";
            ApplySearchFilter();
        }

        private void ShowMyRentals()
        {
            if (!_currentUserId.HasValue)
            {
                MessageBox.Show("Login as client to view your rentals.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PromptClientLoginIfNeeded();
                return;
            }

            _myRentalsOnly = true;
            label5.Text = $"My Rentals - {_currentUsername}";
            ApplySearchFilter();
        }

        private void PromptClientLoginIfNeeded()
        {
            if (_currentUserId.HasValue)
            {
                return;
            }

            SystemRental loginForm = new SystemRental();
            loginForm.Show();
        }

        private static string FormatDate(string? dateText)
        {
            if (DateTime.TryParse(dateText, out DateTime date))
            {
                return date.ToString("MMMM d, yyyy");
            }

            return "-";
        }

        private sealed class ItemRow
        {
            internal ItemRow(Panel panel, Label itemLabel, Label startDateLabel, Label returnDateLabel, Button rentButton)
            {
                Panel = panel;
                ItemLabel = itemLabel;
                StartDateLabel = startDateLabel;
                ReturnDateLabel = returnDateLabel;
                RentButton = rentButton;
            }

            internal Panel Panel { get; }
            internal Label ItemLabel { get; }
            internal Label StartDateLabel { get; }
            internal Label ReturnDateLabel { get; }
            internal Button RentButton { get; }
            internal long? ItemId { get; set; }
            internal bool HasClientRental { get; set; }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
