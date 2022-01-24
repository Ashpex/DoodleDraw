using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint_
{
    public class SizeUpdateEventArgs : EventArgs
    {
        private double width_;
        private double height_;

        public SizeUpdateEventArgs(double width, double height)
        {
            this.width_ = width;
            this.height_ = height;
        }

        public double Width
        {
            get
            {
                return this.width_;
            }
        }

        public double Height
        {
            get
            {
                return this.height_;
            }
        }
    }
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public delegate void SizeUpdateHandler(object sender, SizeUpdateEventArgs e);
        public event SizeUpdateHandler SizeUpdate;

        public ResizeWindow(double width, double height)
        {
            InitializeComponent();

            WidthValue.Text = width.ToString();
            HeightValue.Text = height.ToString();
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void OnClick_OK(object sender, RoutedEventArgs e)
        {
            double width = 0;
            bool check = Double.TryParse(WidthValue.Text, out width);
            if (check == false) return;

            double height = 0;
            check = Double.TryParse(HeightValue.Text, out height);
            if (check == false) return;

            SizeUpdateEventArgs size = new SizeUpdateEventArgs(width, height);
            SizeUpdate(this, size);
            this.Close();
        }

        private void OnClick_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}