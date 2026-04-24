using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RentalBusinessSystem.Data;
using RentalBusinessSystem.Models;

namespace RentalBusinessSystem.Forms
{
    public partial class CustomerPortalForm : Form
    {
        private RentalItemRepository itemRepository;
        private FlowLayoutPanel pnlItemsContainer;

        public CustomerPortalForm()
        {
            InitializeComponent();
            itemRepository = new RentalItemRepository();
        }

        private void CustomerPortalForm_Load(object sender, EventArgs e)
        {
            this.Text = "Customer Portal - Browse Equipment";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(1100, 700);
            LoadAllItems();
        }

        private void LoadAllItems()
        {
            try
            {
                pnlItemsContainer.Controls.Clear();
                var items = itemRepository.GetAllItems();

                foreach (var item in items)
                {
                    Panel pnlItem = CreateItemCard(item);
                    pnlItemsContainer.Controls.Add(pnlItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateItemCard(RentalItem item)
        {
            Panel pnlCard = new Panel();
            pnlCard.Width = 250;
            pnlCard.Height = 300;
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            pnlCard.BackColor = System.Drawing.Color.White;
            pnlCard.Margin = new Padding(10);

            // Item name
            Label lblName = new Label();
            lblName.Text = item.Name;
            lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblName.Location = new System.Drawing.Point(10, 10);
            lblName.Size = new System.Drawing.Size(230, 30);
            lblName.AutoEllipsis = true;

            // Category
            Label lblCategory = new Label();
            lblCategory.Text = "📂 " + item.Category;
            lblCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblCategory.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            lblCategory.Location = new System.Drawing.Point(10, 45);
            lblCategory.Size = new System.Drawing.Size(230, 20);

            // Description
            Label lblDesc = new Label();
            lblDesc.Text = item.Description;
            lblDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblDesc.Location = new System.Drawing.Point(10, 70);
            lblDesc.Size = new System.Drawing.Size(230, 60);
            lblDesc.AutoEllipsis = true;
            lblDesc.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = "$" + item.DailyRate.ToString("F2") + " /day";
            lblPrice.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblPrice.ForeColor = System.Drawing.Color.FromArgb(52, 168, 83);
            lblPrice.Location = new System.Drawing.Point(10, 140);
            lblPrice.Size = new System.Drawing.Size(230, 30);

            // Availability
            Label lblAvail = new Label();
            lblAvail.Text = "✓ Available: " + item.QuantityAvailable;
            lblAvail.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblAvail.ForeColor = item.QuantityAvailable > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblAvail.Location = new System.Drawing.Point(10, 170);
            lblAvail.Size = new System.Drawing.Size(230, 20);

            // Rent Button
            Button btnRent = new Button();
            btnRent.Text = "Rent Now";
            btnRent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnRent.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            btnRent.ForeColor = System.Drawing.Color.White;
            btnRent.Location = new System.Drawing.Point(10, 260);
            btnRent.Size = new System.Drawing.Size(230, 32);
            btnRent.Click += (s, e) =>
            {
                if (item.QuantityAvailable > 0)
                {
                    RentalForm rentalForm = new RentalForm(item);
                    rentalForm.ShowDialog();
                    LoadAllItems();
                }
                else
                {
                    MessageBox.Show("This item is currently unavailable.", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            pnlCard.Controls.Add(btnRent);
            pnlCard.Controls.Add(lblAvail);
            pnlCard.Controls.Add(lblPrice);
            pnlCard.Controls.Add(lblDesc);
            pnlCard.Controls.Add(lblCategory);
            pnlCard.Controls.Add(lblName);

            return pnlCard;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.Panel pnlHeader = new System.Windows.Forms.Panel();
            System.Windows.Forms.Label lblHeader = new System.Windows.Forms.Label();
            System.Windows.Forms.Panel pnlSearch = new System.Windows.Forms.Panel();
            System.Windows.Forms.ComboBox cmbCategory = new System.Windows.Forms.ComboBox();
            System.Windows.Forms.Button btnFilter = new System.Windows.Forms.Button();

            this.pnlItemsContainer = new System.Windows.Forms.FlowLayoutPanel();

            this.SuspendLayout();

            // Header
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(25, 80, 160);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height = 70;
            lblHeader.Text = "🎬 Welcome to Our Equipment Rental Service";
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.White;
            lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            lblHeader.Padding = new System.Windows.Forms.Padding(20, 15, 0, 0);
            pnlHeader.Controls.Add(lblHeader);

            // Search/Filter Panel
            pnlSearch.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.Padding = new System.Windows.Forms.Padding(15);

            cmbCategory.Items.AddRange(new object[] { "All Items", "Camera", "Projector", "Speaker", "Laptop", "Microphone", "Lighting Kit", "Tripod", "Drone", "Tablet", "VR Headset" });
            cmbCategory.SelectedIndex = 0;
            cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbCategory.Location = new System.Drawing.Point(15, 12);
            cmbCategory.Size = new System.Drawing.Size(150, 25);

            btnFilter.Text = "🔍 Filter";
            btnFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnFilter.BackColor = System.Drawing.Color.FromArgb(52, 168, 83);
            btnFilter.ForeColor = System.Drawing.Color.White;
            btnFilter.Location = new System.Drawing.Point(175, 12);
            btnFilter.Size = new System.Drawing.Size(80, 25);

            pnlSearch.Controls.Add(btnFilter);
            pnlSearch.Controls.Add(cmbCategory);

            // Items Container
            this.pnlItemsContainer.AutoScroll = true;
            this.pnlItemsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlItemsContainer.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlItemsContainer.WrapContents = true;
            this.pnlItemsContainer.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.pnlItemsContainer.Padding = new System.Windows.Forms.Padding(10);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.pnlItemsContainer);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
            this.Name = "CustomerPortalForm";
            this.Text = "Customer Portal";
            this.Load += new System.EventHandler(this.CustomerPortalForm_Load);
            this.ResumeLayout(false);
        }
    }
}
