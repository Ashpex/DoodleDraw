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
    }
}
