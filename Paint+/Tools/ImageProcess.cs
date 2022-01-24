using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint_
{
    class ImageProcess
    {
        public static void FloodFill(Canvas MainCanvas, Image MainCanvasImage, Point point, System.Drawing.Color color)
        {
            System.Drawing.Bitmap bitmap = FileSystem.CanvasToBitmap(MainCanvas);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int[] bits = new int[data.Stride / 4 * data.Height];
            Marshal.Copy(data.Scan0, bits, 0, bits.Length);

            LinkedList<System.Drawing.Point> check = new LinkedList<System.Drawing.Point>();
            
            int floodTo = color.ToArgb();
            int floodFrom = bits[(int)point.X + (int)point.Y * data.Stride / 4];
            bits[(int)point.X + (int)point.Y * data.Stride / 4] = floodTo;

            if (floodFrom != floodTo)
            {
                check.AddLast(new System.Drawing.Point((int)point.X, (int)point.Y));
                while (check.Count > 0)
                {
                    System.Drawing.Point cur = check.First.Value;
                    check.RemoveFirst();

                    foreach (System.Drawing.Point off in new System.Drawing.Point[] {
                new  System.Drawing.Point(0, -1), new  System.Drawing.Point(0, 1), 
                new  System.Drawing.Point(-1, 0), new  System.Drawing.Point(1, 0)})
                    {
                        System.Drawing.Point next = new System.Drawing.Point(cur.X + off.X, cur.Y + off.Y);
                        if (next.X >= 0 && next.Y >= 0 &&
                            next.X < data.Width &&
                            next.Y < data.Height)
                        {
                            if (bits[next.X + next.Y * data.Stride / 4] == floodFrom)
                            {
                                check.AddLast(next);
                                bits[next.X + next.Y * data.Stride / 4] = floodTo;
                            }
                        }
                    }
                }
            }

            Marshal.Copy(bits, 0, data.Scan0, bits.Length);
            bitmap.UnlockBits(data);
            MainCanvasImage.Width = MainCanvas.Width;
            MainCanvasImage.Height = MainCanvas.Height;
            MainCanvasImage.Source = BitmapToImageSource(bitmap);
        }

        static public bool SameColor(System.Drawing.Color c1, System.Drawing.Color c2)
        {
            return ((c1.A == c2.A) && (c1.B == c2.B) && (c1.G == c2.G) && (c1.R == c2.R));
        }

        static public ImageSource BitmapToImageSource(System.Drawing.Bitmap bm)
        {
            System.Windows.Media.Imaging.BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bm.Width, bm.Height));
            return b;
        }

        static public void Rubber(int x, int y, int RubberSize, WriteableBitmap image)
        {
            var halfRubberWidth = RubberSize / 2;
            var halfRubberHeight = RubberSize / 2;
            if (x > (image.PixelWidth - halfRubberWidth))
            {
                x = (image.PixelWidth - halfRubberWidth);
            }
            if (y > (image.PixelHeight - halfRubberHeight))
            {
                y = (image.PixelHeight - halfRubberHeight);
            }
            if (x < halfRubberWidth)
            {
                x = halfRubberWidth;
            }
            if (y < halfRubberHeight)
            {
                y = halfRubberHeight;
            }


            int stride = image.PixelWidth * image.Format.BitsPerPixel / 8;
            int byteSize = stride * image.PixelHeight * image.Format.BitsPerPixel / 8;
            var ary = new byte[byteSize];
            image.CopyPixels(ary, stride, 0);

            var curix = ((y - halfRubberWidth) * stride) + ((x - halfRubberHeight) * 4);

            for (var iy = 0; iy < RubberSize; iy++)
            {
                for (var ix = 0; ix < RubberSize; ix++)
                    for (var b = 0; b < 4; b++)
                    {
                        ary[curix] = 0;
                        curix++;
                    }
                curix = curix + stride - (RubberSize * 4);
            }

            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), ary, stride, 0);
        }

        public static System.Drawing.Bitmap BitmapSourceToBitmap(BitmapSource srs)
        {
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
                {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new System.Drawing.Bitmap(btm);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
