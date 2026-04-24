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
        private static readonly Color PendingColor = Color.FromArgb(221, 149, 34);
        private static readonly Color ApprovedColor = Color.FromArgb(39, 132, 201);
        private static readonly Color ReserveColor = Color.FromArgb(30, 152, 88);
        private static readonly Color DisabledColor = Color.FromArgb(119, 132, 148);

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
            textBox1.PlaceholderText = "Find equipment or bookings...";
            ApplyModernDesign();
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
                    string status = string.Empty;

                    row.ItemId = found ? Convert.ToInt64(itemData!["Id"]) : null;

                    if (latestRentalByItem.TryGetValue(itemName, out DataRow? rentalData))
                    {
                        row.StartDateLabel.Text = FormatDate(rentalData["StartDate"]?.ToString());
                        row.ReturnDateLabel.Text = FormatDate(rentalData["EndDate"]?.ToString());

                        status = rentalData["Status"]?.ToString() ?? string.Empty;
                        row.HasClientRental = !string.Equals(status, "Returned", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        row.StartDateLabel.Text = "-";
                        row.ReturnDateLabel.Text = "-";
                        row.HasClientRental = false;
                    }

                    if (!found)
                    {
                        UpdateActionButton(row, "N/A", false, DisabledColor);
                        continue;
                    }

                    if (string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateActionButton(row, "Pending", false, PendingColor);
                    }
                    else if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateActionButton(row, "Approved", false, ApprovedColor);
                    }
                    else if (quantity <= 0)
                    {
                        UpdateActionButton(row, "Out", false, DisabledColor);
                    }
                    else
                    {
                        UpdateActionButton(row, "Reserve", true, ReserveColor);
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
                MessageBox.Show($"Rental request submitted for {row.ItemLabel.Text}. Status: Pending.", "Request Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Rental Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyModernDesign()
        {
            SuspendLayout();

            guna2Panel1.Visible = false;
            BackColor = Color.FromArgb(234, 240, 248);
            ClientSize = new Size(830, 840);
            MinimumSize = new Size(830, 840);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            panel1.BackColor = Color.FromArgb(20, 38, 73);
            panel1.Location = new Point(0, 0);
            panel1.Size = new Size(ClientSize.Width, 62);
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Regular);
            label1.Location = new Point(18, 19);

            ConfigureNavLabel(label2, "Inventory Dashboard", new Point(245, 22));
            ConfigureNavLabel(label3, "Equipment Catalog", new Point(425, 22));
            ConfigureNavLabel(label4, "My Rentals", new Point(585, 22));

            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label6.ForeColor = Color.White;
            label6.Location = new Point(740, 22);

            label5.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Regular);
            label5.ForeColor = Color.FromArgb(35, 47, 66);
            label5.Location = new Point(24, 78);

            panel2.Location = new Point(24, 126);
            panel3.Location = new Point(286, 126);
            panel4.Location = new Point(548, 126);

            StyleMetricCard(panel2, label7, label8, Color.FromArgb(78, 131, 200));
            StyleMetricCard(panel3, label9, label10, Color.FromArgb(83, 171, 132));
            StyleMetricCard(panel4, label11, label12, Color.FromArgb(213, 147, 52));

            textBox1.BackColor = Color.White;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            textBox1.Location = new Point(24, 232);
            textBox1.Size = new Size(560, 38);

            panel6.BackColor = Color.FromArgb(73, 165, 173);
            panel6.Location = new Point(602, 232);
            panel6.Size = new Size(204, 38);

            label13.Dock = DockStyle.Fill;
            label13.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            label13.TextAlign = ContentAlignment.MiddleCenter;
            label13.Text = "Equipment Booking";

            label15.Text = "EQUIPMENT TYPE";
            label16.Text = "RENTAL START";
            label17.Text = "RETURN DATE";
            label18.Text = "ACTION";

            label15.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            label16.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            label17.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            label18.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);

            label15.ForeColor = Color.FromArgb(73, 84, 104);
            label16.ForeColor = Color.FromArgb(73, 84, 104);
            label17.ForeColor = Color.FromArgb(73, 84, 104);
            label18.ForeColor = Color.FromArgb(73, 84, 104);

            label15.Location = new Point(36, 285);
            label16.Location = new Point(274, 285);
            label17.Location = new Point(454, 285);
            label18.Location = new Point(ClientSize.Width - 146, 285);

            int rowStartY = 314;
            for (int index = 0; index < _itemRows.Count; index++)
            {
                StyleItemRow(_itemRows[index], index, rowStartY + (index * 42));
            }

            ResumeLayout();
        }

        private static void ConfigureNavLabel(Label label, string text, Point location)
        {
            label.Text = text;
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.ForeColor = Color.FromArgb(227, 233, 245);
            label.Location = location;
        }

        private static void StyleMetricCard(Panel panel, Label title, Label value, Color accent)
        {
            panel.Size = new Size(250, 86);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;

            title.AutoSize = true;
            title.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            title.ForeColor = Color.FromArgb(61, 72, 90);
            title.Location = new Point(16, 14);

            value.AutoSize = true;
            value.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            value.ForeColor = accent;
            value.Location = new Point(16, 34);
        }

        private void StyleItemRow(ItemRow row, int index, int y)
        {
            row.Panel.Location = new Point(24, y);
            row.Panel.Size = new Size(ClientSize.Width - 48, 40);
            row.Panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            row.Panel.BorderStyle = BorderStyle.FixedSingle;
            row.Panel.BackColor = index % 2 == 0
                ? Color.FromArgb(248, 251, 254)
                : Color.FromArgb(236, 243, 251);

            row.ItemLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);
            row.ItemLabel.ForeColor = Color.FromArgb(32, 41, 56);
            row.ItemLabel.Location = new Point(12, 11);

            row.StartDateLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            row.StartDateLabel.ForeColor = Color.FromArgb(72, 82, 98);
            row.StartDateLabel.Location = new Point(248, 12);

            row.ReturnDateLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            row.ReturnDateLabel.ForeColor = Color.FromArgb(72, 82, 98);
            row.ReturnDateLabel.Location = new Point(428, 12);

            row.RentButton.Size = new Size(98, 26);
            row.RentButton.Location = new Point(row.Panel.Width - 114, 6);
            row.RentButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            row.RentButton.FlatStyle = FlatStyle.Flat;
            row.RentButton.FlatAppearance.BorderSize = 0;
            row.RentButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            row.RentButton.ForeColor = Color.White;
        }

        private static void UpdateActionButton(ItemRow row, string text, bool enabled, Color backColor)
        {
            row.RentButton.Text = text;
            row.RentButton.Enabled = enabled;
            row.RentButton.BackColor = backColor;
            row.RentButton.ForeColor = Color.White;
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
