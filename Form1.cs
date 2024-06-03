using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace rozpoznawanie
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Ustawienia dla dialogu otwierania pliku
            openFileDialog1.InitialDirectory = "C:\\Users\\admin-gr00\\Desktop\\foto";
            openFileDialog1.Filter = "Pliki obrazów (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|Wszystkie pliki (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap originalImage = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Image = originalImage;

                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image=imageToBinary(originalImage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: Nie można wczytać obrazu z dysku. " + ex.Message);
                }
            }
        }
        private Bitmap imageToBinary(Bitmap image)
        {
            Bitmap binaryImage = new Bitmap(image.Width, image.Height);
            int threshold = int.Parse(textBox1.Text);
            Math.Min(255, Math.Max(0, threshold));
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); // konwersja do odcieni szarości
                    Color newColor = Color.FromArgb(pixelColor.A, grayValue, grayValue, grayValue);

                    if (grayValue >= threshold)
                    {
                        binaryImage.SetPixel(x, y, Color.White); // piksel jasny
                    }
                    else
                    {
                        binaryImage.SetPixel(x, y, Color.Black); // piksel ciemny
                    }
                }
            }
            return binaryImage;
        }

        private Bitmap border(Bitmap image)
        {
            Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(gray, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            Mat result = new Mat(src.Size(), MatType.CV_8UC3, Scalar.All(0));
            Scalar color = Scalar.Red;
            int thickness = 2;
            Cv2.DrawContours(result, contours, -1, color, thickness);

            Bitmap resultBitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(result);
            return resultBitmap;
        }

    }
}
