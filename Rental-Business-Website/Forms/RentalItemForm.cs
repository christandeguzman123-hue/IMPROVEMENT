using System;
using System.Windows.Forms;
using RentalBusinessSystem.Data;
using RentalBusinessSystem.Models;

namespace RentalBusinessSystem.Forms
{
    public partial class RentalItemForm : Form
    {
        private RentalItemRepository itemRepository;
        private RentalItem currentItem;
        private bool isEditMode;

        public RentalItemForm()
        {
            InitializeComponent();
            itemRepository = new RentalItemRepository();
            isEditMode = false;
        }

        public RentalItemForm(RentalItem item)
        {
            InitializeComponent();
            itemRepository = new RentalItemRepository();
            currentItem = item;
            isEditMode = true;
        }

        private void RentalItemForm_Load(object sender, EventArgs e)
        {
            this.Text = isEditMode ? "Edit Rental Item" : "Add New Rental Item";
            this.StartPosition = FormStartPosition.CenterParent;

            PopulateCategories();

            if (isEditMode && currentItem != null)
            {
                txtName.Text = currentItem.Name;
                txtDescription.Text = currentItem.Description;
                numDailyRate.Value = currentItem.DailyRate;
                numQuantity.Value = currentItem.QuantityAvailable;
                cmbCategory.SelectedItem = currentItem.Category;
                txtImagePath.Text = currentItem.ImagePath;
            }
        }

        private void PopulateCategories()
        {
            string[] categories = { "Camera", "Projector", "Speaker", "Laptop", "Microphone", "Lighting Kit", "Tripod", "Drone", "Tablet", "VR Headset" };
            cmbCategory.Items.AddRange(categories);
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                RentalItem item = new RentalItem
                {
                    Id = isEditMode ? currentItem.Id : 0,
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    DailyRate = numDailyRate.Value,
                    QuantityAvailable = (int)numQuantity.Value,
                    Category = cmbCategory.SelectedItem.ToString(),
                    ImagePath = txtImagePath.Text,
                    IsActive = true,
                    CreatedDate = isEditMode ? currentItem.CreatedDate : DateTime.Now
                };

                bool success = isEditMode ? itemRepository.UpdateItem(item) : itemRepository.AddItem(item);

                if (success)
                {
                    MessageBox.Show(isEditMode ? "Item updated successfully." : "Item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = openFileDialog.FileName;
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Item name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (numDailyRate.Value <= 0)
            {
                MessageBox.Show("Daily rate must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numDailyRate.Focus();
                return false;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numQuantity.Focus();
                return false;
            }

            if (cmbCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            return true;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.Panel pnlHeader = new System.Windows.Forms.Panel();
            System.Windows.Forms.Label lblHeader = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblName = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblDescription = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblDailyRate = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblQuantity = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblCategory = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblImagePath = new System.Windows.Forms.Label();

            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.numDailyRate = new System.Windows.Forms.NumericUpDown();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.txtImagePath = new System.Windows.Forms.TextBox();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.numDailyRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.SuspendLayout();

            // Header
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(25, 80, 160);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height = 50;
            lblHeader.Text = "Equipment Details";
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.White;
            lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            lblHeader.Padding = new System.Windows.Forms.Padding(15, 10, 0, 0);
            pnlHeader.Controls.Add(lblHeader);

            int yPos = 70;
            int xLabel = 20;
            int xControl = 150;
            int controlWidth = 280;

            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(xLabel, yPos);
            lblName.Text = "Item Name:";
            lblName.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.txtName.Location = new System.Drawing.Point(xControl, yPos);
            this.txtName.Size = new System.Drawing.Size(controlWidth, 28);
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblDescription.AutoSize = true;
            lblDescription.Location = new System.Drawing.Point(xLabel, yPos);
            lblDescription.Text = "Description:";
            lblDescription.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.txtDescription.Location = new System.Drawing.Point(xControl, yPos);
            this.txtDescription.Multiline = true;
            this.txtDescription.Size = new System.Drawing.Size(controlWidth, 70);
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 80;

            lblDailyRate.AutoSize = true;
            lblDailyRate.Location = new System.Drawing.Point(xLabel, yPos);
            lblDailyRate.Text = "Daily Rate ($):";
            lblDailyRate.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.numDailyRate.Location = new System.Drawing.Point(xControl, yPos);
            this.numDailyRate.Maximum = 10000;
            this.numDailyRate.DecimalPlaces = 2;
            this.numDailyRate.Size = new System.Drawing.Size(120, 28);
            this.numDailyRate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numDailyRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblQuantity.AutoSize = true;
            lblQuantity.Location = new System.Drawing.Point(xLabel, yPos);
            lblQuantity.Text = "Quantity:";
            lblQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.numQuantity.Location = new System.Drawing.Point(xControl, yPos);
            this.numQuantity.Maximum = 1000;
            this.numQuantity.Size = new System.Drawing.Size(120, 28);
            this.numQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            yPos += 40;

            lblCategory.AutoSize = true;
            lblCategory.Location = new System.Drawing.Point(xLabel, yPos);
            lblCategory.Text = "Category:";
            lblCategory.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.cmbCategory.Location = new System.Drawing.Point(xControl, yPos);
            this.cmbCategory.Size = new System.Drawing.Size(150, 28);
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);

            yPos += 40;

            lblImagePath.AutoSize = true;
            lblImagePath.Location = new System.Drawing.Point(xLabel, yPos);
            lblImagePath.Text = "Image Path:";
            lblImagePath.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.txtImagePath.Location = new System.Drawing.Point(xControl, yPos);
            this.txtImagePath.Size = new System.Drawing.Size(220, 28);
            this.txtImagePath.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtImagePath.ReadOnly = true;
            this.txtImagePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnBrowseImage.Location = new System.Drawing.Point(xControl + 225, yPos);
            this.btnBrowseImage.Size = new System.Drawing.Size(55, 28);
            this.btnBrowseImage.Text = "Browse...";
            this.btnBrowseImage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowseImage.BackColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);

            yPos += 50;

            this.btnSave.Location = new System.Drawing.Point(xControl, yPos);
            this.btnSave.Size = new System.Drawing.Size(130, 38);
            this.btnSave.Text = "✓ Save";
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(52, 168, 83);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.Location = new System.Drawing.Point(xControl + 140, yPos);
            this.btnCancel.Size = new System.Drawing.Size(130, 38);
            this.btnCancel.Text = "✕ Cancel";
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, yPos + 60);
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBrowseImage);
            this.Controls.Add(this.txtImagePath);
            this.Controls.Add(lblImagePath);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(lblCategory);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(this.numDailyRate);
            this.Controls.Add(lblDailyRate);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(lblDescription);
            this.Controls.Add(this.txtName);
            this.Controls.Add(lblName);
            this.Name = "RentalItemForm";
            this.Text = "Rental Item";
            this.Load += new System.EventHandler(this.RentalItemForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numDailyRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.NumericUpDown numDailyRate;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.TextBox txtImagePath;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
