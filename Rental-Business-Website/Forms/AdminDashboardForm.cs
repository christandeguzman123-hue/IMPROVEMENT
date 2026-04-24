using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RentalBusinessSystem.Data;
using RentalBusinessSystem.Models;

namespace RentalBusinessSystem.Forms
{
    public partial class AdminDashboardForm : Form
    {
        private RentalItemRepository itemRepository;
        private RentalRepository rentalRepository;

        public AdminDashboardForm()
        {
            InitializeComponent();
            itemRepository = new RentalItemRepository();
            rentalRepository = new RentalRepository();
        }

        private void AdminDashboardForm_Load(object sender, EventArgs e)
        {
            this.Text = "Rental Business System - Admin Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadRentalItems();
                LoadRentals();
                UpdateDashboardStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRentalItems()
        {
            try
            {
                var items = itemRepository.GetAllItems();
                dgvItems.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRentals()
        {
            try
            {
                var rentals = rentalRepository.GetAllRentals();
                dgvRentals.DataSource = rentals;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rentals: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDashboardStats()
        {
            try
            {
                var items = itemRepository.GetAllItems();
                var rentals = rentalRepository.GetAllRentals();
                lblTotalItems.Text = "Total Items: " + items.Count;
                lblTotalRentals.Text = "Total Rentals: " + rentals.Count;
            }
            catch { }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            RentalItemForm itemForm = new RentalItemForm();
            if (itemForm.ShowDialog() == DialogResult.OK)
            {
                LoadRentalItems();
                UpdateDashboardStats();
            }
        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvItems.SelectedRows[0];
            int itemId = (int)selectedRow.Cells["Id"].Value;
            var item = itemRepository.GetItemById(itemId);

            if (item != null)
            {
                RentalItemForm itemForm = new RentalItemForm(item);
                if (itemForm.ShowDialog() == DialogResult.OK)
                {
                    LoadRentalItems();
                    UpdateDashboardStats();
                }
            }
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var selectedRow = dgvItems.SelectedRows[0];
                    int itemId = (int)selectedRow.Cells["Id"].Value;
                    itemRepository.DeleteItem(itemId);
                    LoadRentalItems();
                    UpdateDashboardStats();
                    MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AdminLoginForm loginForm = new AdminLoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("Dashboard refreshed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.Panel pnlHeader = new System.Windows.Forms.Panel();
            System.Windows.Forms.Label lblHeader = new System.Windows.Forms.Label();
            System.Windows.Forms.FlowLayoutPanel pnlStats = new System.Windows.Forms.FlowLayoutPanel();
            System.Windows.Forms.Label lblItemsCount = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblRentalsCount = new System.Windows.Forms.Label();
            System.Windows.Forms.TabControl tabControl = new System.Windows.Forms.TabControl();
            System.Windows.Forms.TabPage tabItems = new System.Windows.Forms.TabPage();
            System.Windows.Forms.TabPage tabRentals = new System.Windows.Forms.TabPage();
            System.Windows.Forms.Panel pnlBottom = new System.Windows.Forms.Panel();

            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.dgvRentals = new System.Windows.Forms.DataGridView();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblTotalRentals = new System.Windows.Forms.Label();

            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnEditItem = new System.Windows.Forms.Button();
            this.btnDeleteItem = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRentals)).BeginInit();
            this.SuspendLayout();

            // Header
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(25, 80, 160);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height = 60;
            lblHeader.Text = "📊 Equipment Dashboard";
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.White;
            lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            lblHeader.Padding = new System.Windows.Forms.Padding(20, 15, 0, 0);
            pnlHeader.Controls.Add(lblHeader);

            // Statistics Panel
            pnlStats.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);
            pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            pnlStats.Height = 100;
            pnlStats.Padding = new System.Windows.Forms.Padding(20);

            lblItemsCount.Text = "📦\n0\nItems";
            lblItemsCount.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            lblItemsCount.Width = 130;
            lblItemsCount.Height = 80;
            lblItemsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblItemsCount.BackColor = System.Drawing.Color.FromArgb(52, 168, 83);
            lblItemsCount.ForeColor = System.Drawing.Color.White;
            pnlStats.Controls.Add(lblItemsCount);

            lblRentalsCount.Text = "🔄\n0\nRentals";
            lblRentalsCount.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            lblRentalsCount.Width = 130;
            lblRentalsCount.Height = 80;
            lblRentalsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblRentalsCount.BackColor = System.Drawing.Color.FromArgb(255, 152, 0);
            lblRentalsCount.ForeColor = System.Drawing.Color.White;
            pnlStats.Controls.Add(lblRentalsCount);

            this.lblTotalItems = lblItemsCount;
            this.lblTotalRentals = lblRentalsCount;

            // Tab Control
            tabControl.Controls.Add(tabItems);
            tabControl.Controls.Add(tabRentals);
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);

            // Tab Item Management
            tabItems.Text = "Rental Items";
            tabItems.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            tabItems.Controls.Add(this.dgvItems);
            tabItems.Controls.Add(new System.Windows.Forms.Panel() { Height = 50, Dock = System.Windows.Forms.DockStyle.Bottom });
            
            var pnlItemButtons = tabItems.Controls[tabItems.Controls.Count - 1] as System.Windows.Forms.Panel;
            pnlItemButtons.BackColor = System.Drawing.Color.White;
            pnlItemButtons.Controls.Add(this.btnAddItem);
            pnlItemButtons.Controls.Add(this.btnEditItem);
            pnlItemButtons.Controls.Add(this.btnDeleteItem);

            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.ReadOnly = true;

            this.btnAddItem.BackColor = System.Drawing.Color.FromArgb(52, 168, 83);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.Location = new System.Drawing.Point(10, 12);
            this.btnAddItem.Size = new System.Drawing.Size(110, 30);
            this.btnAddItem.Text = "+ Add Item";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);

            this.btnEditItem.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnEditItem.ForeColor = System.Drawing.Color.White;
            this.btnEditItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEditItem.Location = new System.Drawing.Point(130, 12);
            this.btnEditItem.Size = new System.Drawing.Size(110, 30);
            this.btnEditItem.Text = "✏️ Edit";
            this.btnEditItem.Click += new System.EventHandler(this.btnEditItem_Click);

            this.btnDeleteItem.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnDeleteItem.ForeColor = System.Drawing.Color.White;
            this.btnDeleteItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDeleteItem.Location = new System.Drawing.Point(250, 12);
            this.btnDeleteItem.Size = new System.Drawing.Size(110, 30);
            this.btnDeleteItem.Text = "🗑️ Delete";
            this.btnDeleteItem.Click += new System.EventHandler(this.btnDeleteItem_Click);

            // Tab Rentals
            tabRentals.Text = "Active Rentals";
            tabRentals.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            tabRentals.Controls.Add(this.dgvRentals);

            this.dgvRentals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRentals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRentals.AllowUserToAddRows = false;
            this.dgvRentals.ReadOnly = true;

            // Bottom Panel
            pnlBottom.BackColor = System.Drawing.Color.White;
            pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlBottom.Height = 50;
            pnlBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(156, 39, 176);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(10, 10);
            this.btnRefresh.Size = new System.Drawing.Size(100, 32);
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.Location = new System.Drawing.Point(950, 10);
            this.btnLogout.Size = new System.Drawing.Size(100, 32);
            this.btnLogout.Text = "🚪 Logout";
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            pnlBottom.Controls.Add(this.btnRefresh);
            pnlBottom.Controls.Add(this.btnLogout);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 650);
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Controls.Add(tabControl);
            this.Controls.Add(pnlStats);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBottom);
            this.Name = "AdminDashboardForm";
            this.Text = "Admin Dashboard - Rental Equipment Management";
            this.Load += new System.EventHandler(this.AdminDashboardForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRentals)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridView dgvRentals;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.Label lblTotalRentals;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnEditItem;
        private System.Windows.Forms.Button btnDeleteItem;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnLogout;
    }
}
