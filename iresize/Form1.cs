using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace iresize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void label1_Click(object sender, EventArgs e)
        {
            //select folder dialog
            int imagestotal = 0;
            string currtime = string.Empty;
            DateTime now;

            DialogResult result = MessageBox.Show("Scaling process ill be started immediately using selected parameters. Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = dialog.SelectedPath;
                    now = DateTime.Now;
                    currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff\n");

                    textBox2.AppendText("Operation Started ");
                    textBox2.AppendText(currtime);

                    String searchFolder = textBox1.Text;
                    var filters = new String[] { "jpg", "jpeg"};
                    var files = GetFilesFrom(searchFolder, filters, false);
                    foreach(string jpgfile in files)
                    {
                        imagestotal++;
                        now = DateTime.Now;
                        currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff\n");
                        textBox2.AppendText("\n" + jpgfile + " pocessing started at " + currtime );

                        Image imgPhotoVert = Image.FromFile(jpgfile);
                        int scale = trackBar1.Value;
                        Image imgPhoto = ScaleByPercent(imgPhotoVert, scale);

                        string newImage = (System.IO.Path.ChangeExtension(jpgfile, null) + "_resized" + System.IO.Path.GetExtension(jpgfile));
                        imgPhoto.Save(newImage, ImageFormat.Jpeg);
                        imgPhotoVert.Dispose();
                        imgPhoto.Dispose();

                        now = DateTime.Now;
                        currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff\n");
                        textBox2.AppendText(newImage + " pocessing finished at " + currtime);
                    }
                    if(checkBox1.Checked == true)
                    {
                        foreach (string jpgfile in files)
                        {
                            textBox2.AppendText("Deleting file: " + jpgfile + "\n");
                            File.Delete(jpgfile);
                        }
                    }
                    now = DateTime.Now;
                    currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff\n");
                    textBox2.AppendText("All operations finished at " + currtime);
                }
                else
                {
                    now = DateTime.Now;
                    currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff");
                    textBox2.AppendText("Operation Cancelled ");
                    textBox2.AppendText(currtime);
                }
            }
            else
            {
                now = DateTime.Now;
                currtime = now.ToString("MM'/'dd'/'yyyy HH':'mm':'ss.fff");
                textBox2.AppendText("Operation Cancelled ");
                textBox2.AppendText(currtime);
            }

        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = trackBar1.Value.ToString();
        }
        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }
        public static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight),new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
