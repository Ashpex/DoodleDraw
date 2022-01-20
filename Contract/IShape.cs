using System;
using System.Windows;
using System.Windows.Controls;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);
        void setColor(System.Windows.Media.Color color); //set màu 
        void setWidth(int widthStroke); // set độ dày 
        void setStyle(string style); // set style
        Point2D GetPoint1();
        Point2D GetPoint2();

        void DrawMove(Canvas canvas);

        UIElement Draw();
        IShape Clone();

        String toString();
    }
}