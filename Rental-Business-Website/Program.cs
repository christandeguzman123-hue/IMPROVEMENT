using System;
using System.Windows.Forms;
using RentalBusinessSystem.Forms;
using RentalBusinessSystem.Data;

namespace RentalBusinessSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check database connection
            if (!DatabaseConnection.TestConnection())
            {
                MessageBox.Show(
                    "Failed to connect to the database. Please ensure:\n" +
                    "1. XAMPP MySQL server is running\n" +
                    "2. Database 'rental_business_db' exists\n" +
                    "3. Username is 'root' with no password",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            try
            {
                Application.Run(new AdminLoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
