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
    public partial class SystemRental : Form
    {
        private readonly AuthService _authService = new(SqliteDatabase.ConnectionString);

        public SystemRental()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {
            SystemBusiness Form1 = new SystemBusiness();
            Form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter your email and password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserSession? session = _authService.TryLogin(textBox1.Text, textBox2.Text, "Client");
            if (session is null)
            {
                MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Welcome!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Business clientDashboard = new Business(session.UserId, session.Username);
            clientDashboard.FormClosed += (_, _) => Show();
            Hide();
            clientDashboard.Show(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Enter email and password to register.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            (bool success, string message) = _authService.RegisterClient(textBox1.Text, textBox2.Text);
            MessageBoxIcon icon = success ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
            MessageBox.Show(message, success ? "Registration" : "Registration Failed", MessageBoxButtons.OK, icon);
        }
    }
}
