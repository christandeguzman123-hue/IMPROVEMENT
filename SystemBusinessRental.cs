using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rental_Business_System
{
    public partial class SystemBusinessRental : Form
    {
        public SystemBusinessRental()
        {
            InitializeComponent();
            EnsurePanelImages();
        }

        private void EnsurePanelImages()
        {
            panel2.BackgroundImage = LoadImageFromImgFolder("628339029_1227535559535842_6292811212887707502_n.jpg")
                ?? panel2.BackgroundImage;
            panel3.BackgroundImage = LoadImageFromImgFolder("663602135_1645082033053893_6330011561762422357_n.jpg")
                ?? panel3.BackgroundImage;

            panel1.BackgroundImage ??= CreatePlaceholderImage(panel1.Size, "Member");
            panel2.BackgroundImage ??= CreatePlaceholderImage(panel2.Size, "Member");
            panel3.BackgroundImage ??= CreatePlaceholderImage(panel3.Size, "Member");
            panel4.BackgroundImage ??= CreatePlaceholderImage(panel4.Size, "Member");
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
            int width = Math.Max(size.Width, 64);
            int height = Math.Max(size.Height, 64);
            Bitmap bitmap = new Bitmap(width, height);

            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.LightSteelBlue);
            using Pen borderPen = new Pen(Color.SteelBlue, 3);
            g.DrawRectangle(borderPen, 1, 1, width - 2, height - 2);

            using Font font = new Font("Segoe UI", Math.Max(10, height / 9f), FontStyle.Bold, GraphicsUnit.Pixel);
            using StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(text, font, Brushes.SteelBlue, new RectangleF(0, 0, width, height), format);
            return bitmap;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
