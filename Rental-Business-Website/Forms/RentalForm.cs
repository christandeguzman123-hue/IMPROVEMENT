using System;
using System.Windows.Forms;
using RentalBusinessSystem.Data;
using RentalBusinessSystem.Models;
using RentalBusinessSystem.Utilities;

namespace RentalBusinessSystem.Forms
{
    public partial class RentalForm : Form
    {
        private RentalRepository rentalRepository;
        private RentalItemRepository itemRepository;
        private RentalItem selectedItem;

        public RentalForm(RentalItem item)
        {
            InitializeComponent();
            rentalRepository = new RentalRepository();
            itemRepository = new RentalItemRepository();
            selectedItem = item;
        }

        private void RentalForm_Load(object sender, EventArgs e)
        {
            this.Text = "Create Rental - " + selectedItem.Name;
            this.StartPosition = FormStartPosition.CenterParent;

            lblItemName.Text = selectedItem.Name;
            lblDailyRate.Text = "$" + selectedItem.DailyRate.ToString("F2");
            lblAvailableQuantity.Text = selectedItem.QuantityAvailable.ToString();

            PopulatePaymentMethods();
            dtpStartDate.MinDate = DateTime.Today;
            dtpEndDate.MinDate = DateTime.Today.AddDays(1);

            numQuantity.Maximum = selectedItem.QuantityAvailable;
        }

        private void PopulatePaymentMethods()
        {
            cmbPaymentMethod.DataSource = PaymentProcessor.PaymentMethods.GetAllPaymentMethods();
            if (cmbPaymentMethod.Items.Count > 0)
                cmbPaymentMethod.SelectedIndex = 0;
        }

        private void numQuantity_ValueChanged(object sender, EventArgs e)
        {
            CalculateCost();
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            dtpEndDate.MinDate = dtpStartDate.Value.AddDays(1);
            CalculateCost();
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            CalculateCost();
        }

        private void CalculateCost()
        {
            int days = (dtpEndDate.Value - dtpStartDate.Value).Days;
            if (days <= 0) days = 1;

            decimal totalCost = PaymentProcessor.CalculateTotalCost(
                selectedItem.DailyRate,
                days,
                (int)numQuantity.Value
            );

            lblTotalCost.Text = "$" + totalCost.ToString("F2");
        }

        private void btnCreateRental_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                int days = (dtpEndDate.Value - dtpStartDate.Value).Days;
                if (days <= 0) days = 1;

                decimal totalCost = PaymentProcessor.CalculateTotalCost(
                    selectedItem.DailyRate,
                    days,
                    (int)numQuantity.Value
                );

                Rental rental = new Rental
                {
                    ItemId = selectedItem.Id,
                    CustomerName = txtCustomerName.Text,
                    CustomerEmail = txtCustomerEmail.Text,
                    CustomerPhone = txtCustomerPhone.Text,
                    RentalStartDate = dtpStartDate.Value,
                    RentalEndDate = dtpEndDate.Value,
                    Quantity = (int)numQuantity.Value,
                    TotalCost = totalCost,
                    PaymentMethod = cmbPaymentMethod.SelectedItem.ToString(),
                    PaymentStatus = "Pending",
                    RentalStatus = "Active",
                    CreatedDate = DateTime.Now
                };

                if (rentalRepository.CreateRental(rental))
                {
                    MessageBox.Show("Rental created successfully. Total: $" + totalCost.ToString("F2"), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create rental.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating rental: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Customer name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerEmail.Text))
            {
                MessageBox.Show("Customer email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerPhone.Text))
            {
                MessageBox.Show("Customer phone is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerPhone.Focus();
                return false;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpEndDate.Value <= dtpStartDate.Value)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.Panel pnlHeader = new System.Windows.Forms.Panel();
            System.Windows.Forms.Label lblHeader = new System.Windows.Forms.Label();
            System.Windows.Forms.Panel pnlItemInfo = new System.Windows.Forms.Panel();
            System.Windows.Forms.Label lblItem = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblRate = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblAvailable = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblCustomer = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblEmail = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblPhone = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblQuantity = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblStart = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblEnd = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblPayment = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblTotal = new System.Windows.Forms.Label();

            this.lblItemName = new System.Windows.Forms.Label();
            this.lblDailyRate = new System.Windows.Forms.Label();
            this.lblAvailableQuantity = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerEmail = new System.Windows.Forms.TextBox();
            this.txtCustomerPhone = new System.Windows.Forms.TextBox();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblTotalCost = new System.Windows.Forms.Label();
            this.btnCreateRental = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.SuspendLayout();

            // Header
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(25, 80, 160);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height = 50;
            lblHeader.Text = "Create New Rental";
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.White;
            lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            lblHeader.Padding = new System.Windows.Forms.Padding(15, 10, 0, 0);
            pnlHeader.Controls.Add(lblHeader);

            // Item Info Panel
            pnlItemInfo.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);
            pnlItemInfo.Dock = System.Windows.Forms.DockStyle.Top;
            pnlItemInfo.Height = 100;
            pnlItemInfo.Padding = new System.Windows.Forms.Padding(15);
            pnlItemInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            lblItem.Text = "Equipment:";
            lblItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblItem.Location = new System.Drawing.Point(15, 15);
            pnlItemInfo.Controls.Add(lblItem);

            this.lblItemName.Text = "";
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblItemName.Location = new System.Drawing.Point(150, 15);
            this.lblItemName.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            pnlItemInfo.Controls.Add(this.lblItemName);

            lblRate.Text = "Daily Rate:";
            lblRate.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblRate.Location = new System.Drawing.Point(15, 40);
            pnlItemInfo.Controls.Add(lblRate);

            this.lblDailyRate.Text = "$0.00";
            this.lblDailyRate.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDailyRate.Location = new System.Drawing.Point(150, 40);
            this.lblDailyRate.ForeColor = System.Drawing.Color.FromArgb(52, 168, 83);
            pnlItemInfo.Controls.Add(this.lblDailyRate);

            lblAvailable.Text = "Available:";
            lblAvailable.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblAvailable.Location = new System.Drawing.Point(15, 65);
            pnlItemInfo.Controls.Add(lblAvailable);

            this.lblAvailableQuantity.Text = "0";
            this.lblAvailableQuantity.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAvailableQuantity.Location = new System.Drawing.Point(150, 65);
            pnlItemInfo.Controls.Add(this.lblAvailableQuantity);

            int yPos = 70;
            int xLabel = 20;
            int xControl = 150;
            int controlWidth = 300;

            lblCustomer.Text = "Customer Name:";
            lblCustomer.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblCustomer.Location = new System.Drawing.Point(xLabel, yPos);

            this.txtCustomerName.Location = new System.Drawing.Point(xControl, yPos);
            this.txtCustomerName.Size = new System.Drawing.Size(controlWidth, 28);
            this.txtCustomerName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCustomerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblEmail.Text = "Email:";
            lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblEmail.Location = new System.Drawing.Point(xLabel, yPos);

            this.txtCustomerEmail.Location = new System.Drawing.Point(xControl, yPos);
            this.txtCustomerEmail.Size = new System.Drawing.Size(controlWidth, 28);
            this.txtCustomerEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCustomerEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblPhone.Text = "Phone:";
            lblPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblPhone.Location = new System.Drawing.Point(xLabel, yPos);

            this.txtCustomerPhone.Location = new System.Drawing.Point(xControl, yPos);
            this.txtCustomerPhone.Size = new System.Drawing.Size(controlWidth, 28);
            this.txtCustomerPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCustomerPhone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblQuantity.Text = "Quantity:";
            lblQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblQuantity.Location = new System.Drawing.Point(xLabel, yPos);

            this.numQuantity.Location = new System.Drawing.Point(xControl, yPos);
            this.numQuantity.Size = new System.Drawing.Size(120, 28);
            this.numQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numQuantity.Value = 1;
            this.numQuantity.ValueChanged += new System.EventHandler(this.numQuantity_ValueChanged);

            yPos += 40;

            lblStart.Text = "Start Date:";
            lblStart.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblStart.Location = new System.Drawing.Point(xLabel, yPos);

            this.dtpStartDate.Location = new System.Drawing.Point(xControl, yPos);
            this.dtpStartDate.Size = new System.Drawing.Size(250, 28);
            this.dtpStartDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpStartDate.ValueChanged += new System.EventHandler(this.dtpStartDate_ValueChanged);

            yPos += 40;

            lblEnd.Text = "End Date:";
            lblEnd.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblEnd.Location = new System.Drawing.Point(xLabel, yPos);

            this.dtpEndDate.Location = new System.Drawing.Point(xControl, yPos);
            this.dtpEndDate.Size = new System.Drawing.Size(250, 28);
            this.dtpEndDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpEndDate.ValueChanged += new System.EventHandler(this.dtpEndDate_ValueChanged);

            yPos += 40;

            lblPayment.Text = "Payment Method:";
            lblPayment.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblPayment.Location = new System.Drawing.Point(xLabel, yPos);

            this.cmbPaymentMethod.Location = new System.Drawing.Point(xControl, yPos);
            this.cmbPaymentMethod.Size = new System.Drawing.Size(200, 28);
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 10F);

            yPos += 50;

            lblTotal.Text = "Total Cost:";
            lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblTotal.Location = new System.Drawing.Point(xLabel, yPos);
            lblTotal.ForeColor = System.Drawing.Color.FromArgb(244, 67, 54);

            this.lblTotalCost.Text = "$0.00";
            this.lblTotalCost.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalCost.Location = new System.Drawing.Point(xControl, yPos);
            this.lblTotalCost.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.lblTotalCost.AutoSize = true;

            yPos += 50;

            this.btnCreateRental.Location = new System.Drawing.Point(xControl, yPos);
            this.btnCreateRental.Size = new System.Drawing.Size(150, 40);
            this.btnCreateRental.Text = "✓ Create Rental";
            this.btnCreateRental.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCreateRental.BackColor = System.Drawing.Color.FromArgb(52, 168, 83);
            this.btnCreateRental.ForeColor = System.Drawing.Color.White;
            this.btnCreateRental.Click += new System.EventHandler(this.btnCreateRental_Click);

            this.btnCancel.Location = new System.Drawing.Point(xControl + 160, yPos);
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.Text = "✕ Cancel";
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, yPos + 70);
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlItemInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateRental);
            this.Controls.Add(this.lblTotalCost);
            this.Controls.Add(lblTotal);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(lblPayment);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(lblEnd);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(lblStart);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(this.txtCustomerPhone);
            this.Controls.Add(lblPhone);
            this.Controls.Add(this.txtCustomerEmail);
            this.Controls.Add(lblEmail);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(lblCustomer);
            this.Name = "RentalForm";
            this.Text = "Create Rental";
            this.Load += new System.EventHandler(this.RentalForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblDailyRate;
        private System.Windows.Forms.Label lblAvailableQuantity;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.TextBox txtCustomerEmail;
        private System.Windows.Forms.TextBox txtCustomerPhone;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblTotalCost;
        private System.Windows.Forms.Button btnCreateRental;
        private System.Windows.Forms.Button btnCancel;
    }
}
