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
    public partial class BusinessSystem : Form
    {
        private readonly AuthService _authService = new(SqliteDatabase.ConnectionString);

        public BusinessSystem()
        {
            InitializeComponent();
            BtntextBox2.UseSystemPasswordChar = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BtntextBox1.Text) || string.IsNullOrWhiteSpace(BtntextBox2.Text))
            {
                MessageBox.Show("Please enter all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserSession? session = _authService.TryLogin(BtntextBox1.Text, BtntextBox2.Text, "Admin");
            if (session is not null)
            {
                MessageBox.Show("Login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RentalManagementForm managementForm = new RentalManagementForm(session);
                managementForm.FormClosed += (_, _) => Show();
                Hide();
                managementForm.Show(this);
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtntextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void BusinessSystem_Load(object sender, EventArgs e)
        {

        }
    }
}
