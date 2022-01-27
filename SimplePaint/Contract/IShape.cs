using System;
using System.Windows;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);

        UIElement Draw();
        IShape Clone();
    }
}
