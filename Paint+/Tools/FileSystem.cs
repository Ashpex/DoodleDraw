using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint_
{
    public class FileSystem
    {
        public static List<string> pathTemp = new List<string>();

        public static void AddTemp(string temp)
        {
            pathTemp.Add(temp);
        }

        public static void ClearTemp()
        {
            foreach(string str in pathTemp){
                try
                {
                    File.Delete(str);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Lỗi ClearTemp!");
                }
            }
            
        }
        public static string OpenFile(){
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "Choose an image file";
            dlg.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*";
            try
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dlg.Dispose();
                    return dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk.\nOriginal error: " + ex.Message);
            }
            return "";
        }

        public static void SaveFile(Canvas canvas)
        {
            canvas.Background = System.Windows.Media.Brushes.Transparent;
            canvas.UpdateLayout();
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);

                ms.Close();
                ms.Dispose();
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = "Save as";
                dlg.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileName = dlg.FileName;
                    System.IO.File.WriteAllBytes(fileName, ms.ToArray());
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            canvas.Background = System.Windows.Media.Brushes.White;
        }

        public static string CanvasToFileTemp(Canvas canvas)
        {
            canvas.Background = System.Windows.Media.Brushes.Transparent;
            canvas.UpdateLayout();
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);

                ms.Close();
                ms.Dispose();
                string fileName = CreateTempFile();
                System.IO.File.WriteAllBytes(fileName, ms.ToArray());
                canvas.Background = System.Windows.Media.Brushes.White;
                return fileName;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            canvas.Background = System.Windows.Media.Brushes.White;
            return "";
        }

        public static Bitmap CanvasToBitmap(Canvas cv)
        {
            Bitmap bm;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(cv);
            double dpi = 96d;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);


            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(cv);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            renderBitmap.Render(dv);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
            bm = new System.Drawing.Bitmap(stream);
            return bm;
        }

        public static string CreateTempFile()
        {
            string fileName = string.Empty;

            try
            {
                fileName = System.IO.Path.GetTempFileName();

                // Create a FileInfo object to set the file's attributes
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);

                // Set the Attribute property of this file to Temporary. 
                fileInfo.Attributes = System.IO.FileAttributes.Temporary;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create tempfile\nDetail: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return fileName;
        }
    }
}
