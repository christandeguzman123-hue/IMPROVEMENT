namespace Rental_Business_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
      
        }

        private static Image? LoadImageFromImgFolder(string fileName)
        {
            string imagePath = Path.Combine(AppContext.BaseDirectory, "img", fileName);
            if (!File.Exists(imagePath))
            {
                return null;
            }

            try
            {
                return Image.FromFile(imagePath);
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap CreatePlaceholderImage(Size size, string text)
        {
            int width = Math.Max(size.Width, 32);
            int height = Math.Max(size.Height, 32);
            Bitmap bitmap = new Bitmap(width, height);

            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.LightGray);
            using Pen borderPen = new Pen(Color.DimGray, 2);
            g.DrawRectangle(borderPen, 1, 1, width - 2, height - 2);

            using Font font = new Font("Segoe UI", Math.Max(8, height / 6f), FontStyle.Bold, GraphicsUnit.Pixel);
            using StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(text, font, Brushes.DimGray, new RectangleF(0, 0, width, height), format);
            return bitmap;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SystemRental());
        }

        private void label7_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BusinessSystem());
        }

        private void label5_Click(object sender, EventArgs e)
        {
            SystemBusinessRental Form1 = new SystemBusinessRental();
            Form1.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SystemRental());
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Form2 Form1 = new Form2();
            Form1.Show();
        }
        private void label9_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BusinessSystem());
        }

        private void OpenChildForm(Form child)
        {
            child.StartPosition = FormStartPosition.CenterScreen;
            child.FormClosed += (_, _) => Show();
            Hide();
            child.Show(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {
            SystemBusinessRental Form1 = new SystemBusinessRental();
            Form1.Show();
            ButtonOff();
            BtnAbout.FillColor = Color.FromArgb(50, 100, 201);
            BtnAbout.FillColor = Color.FromArgb(144, 117, 203);
        }

        private void BtnEquipment_Click(object sender, EventArgs e)
        {
            Form2 Form1 = new Form2();
            Form1.Show();
            ButtonOff();
            BtnEquipment.FillColor = Color.FromArgb(50, 100, 201);
            BtnEquipment.FillColor = Color.FromArgb(144, 117, 203);
        }
        private void ButtonOff()
        {
            BtnEquipment.FillColor = Color.Transparent;
            BtnEquipment.FillColor = Color.Transparent;

            BtnAbout.FillColor = Color.Transparent;
            BtnAbout.FillColor = Color.Transparent;

            BtnRental.FillColor = Color.Transparent;
            BtnRental.FillColor = Color.Transparent;

            BtnLogin.FillColor = Color.Transparent;
            BtnLogin.FillColor = Color.Transparent;

            BtnAdmin.FillColor = Color.Transparent;
            BtnAdmin.FillColor = Color.Transparent;
        }

        private void BtnRental_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SystemRental());
            ButtonOff();
            BtnRental.FillColor = Color.FromArgb(50, 100, 201);
            BtnRental.FillColor = Color.FromArgb(144, 117, 203);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SystemRental());
            ButtonOff();
            BtnLogin.FillColor = Color.FromArgb(50, 100, 201);
            BtnLogin.FillColor = Color.FromArgb(144, 117, 203);
        }

        private void BtnAdmin_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BusinessSystem());
            ButtonOff();
            BtnAdmin.FillColor = Color.FromArgb(50, 100, 201);
            BtnAdmin.FillColor = Color.FromArgb(144, 117, 203);
        }
    }
}

