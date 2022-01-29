using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        
        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);

        UIElement Draw();
        IShape Clone();

        public void setValue(Color color, double strokeThickness, double border);
        public void getValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border);
        public void setValueSave(ref Color color, ref Point2D leftTop, ref Point2D rightBottom, ref double strokeThickness, ref double border);
    }
}
