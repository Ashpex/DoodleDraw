using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjectPaint.Adorners
{
    public static class CanvasUltilities
    {
        //Tham khảo Nguyễn Minh Tuấn (Tuấn Mập)
        public static Image Crop(Panel displayer, double x, double y, double Width, double Height)
        {
            if (displayer == null) return null;

            Rect rect = new Rect(displayer.RenderSize);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
              (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(displayer);

            if (Width < 4 || Height < 4) return null;
            var crop = new CroppedBitmap(rtb, new Int32Rect((int)x, (int)y, (int)Width, (int)Height));

            return new Image() { Source = BitmapFrame.Create(crop) };
        }
    }
}
