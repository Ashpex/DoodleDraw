using Contract;
using Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimplePaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
            KeyUp += new KeyEventHandler(OnButtonKeyUp);
        }
        bool shiftMode;
        bool ctrlMode;

        private void OnButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = false;
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = true;
            }
            else
          if (e.Key == Key.Z && ctrlMode)
            {
                if (_shapes.Count > 0)
                {
                    _redos.Add(_shapes[_shapes.Count - 1]);
                    _shapes.RemoveAt(_shapes.Count - 1);
                }
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            else if (e.Key == Key.Y && ctrlMode)
            {
                if (_redos.Count > 0)
                {
                    _shapes.Add(_redos[_redos.Count - 1]);
                    canvas.Children.Add(_shapes[_shapes.Count - 1].Draw());
                    _redos.RemoveAt(_redos.Count - 1);
                }
            }
        }

        List<IShape> _shapes = new List<IShape>();
        List<IShape> _redos = new List<IShape>();
        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        string _selectedShapeName = "";
        bool _isDrawing = false;
        IShape _preview;
        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                

                string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = System.IO.Path.GetDirectoryName(exePath);
                var dlls = new DirectoryInfo(folder).GetFiles("*.dll");

                foreach (var dll in dlls)
                {
                    //if(dll.FullName== @$"{folder}\ControlzEx.dll")
                    //{
                    //    //var domain = AppDomain.CurrentDomain;
                    //    //Assembly assembly1 = domain.Load(AssemblyName.GetAssemblyName(dll.FullName));
                    //    continue;
                    //}

                    //var domain = AppDomain.CurrentDomain;
                    //Assembly assembly = domain.Load(AssemblyName.GetAssemblyName(dll.FullName));
                    //var types = assembly.GetTypes();

                    var assembly = Assembly.LoadFrom(dll.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsClass)
                        {
                            if (typeof(IShape).IsAssignableFrom(type))
                            {
                                var shape = Activator.CreateInstance(type) as IShape;
                                _prototypes.Add(shape.Name, shape);
                            }
                        }
                    }
                    _prototypes.Remove("Point");
                    _preview = _prototypes.First().Value.Clone();
                    methodComboBox.ItemsSource = _prototypes;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            _isDrawing = true;

            Point pos = e.GetPosition(canvas);
            if (_preview != null)
            {
                System.Drawing.Color getColor = System.Drawing.Color.FromName(colorComboBox.Text);
                //_preview.Color = Color.FromArgb(getColor.A, getColor.R, getColor.G, getColor.B);
                double size = double.Parse(sizeComboBox.Text);
                double border = double.Parse(borderComboBox.Text);
                _preview.setValue(Color.FromArgb(getColor.A, getColor.R, getColor.G, getColor.B), size, border);
                _preview.HandleStart(pos.X, pos.Y);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
          
            if (_isDrawing)
            {
                if (_preview != null)
                {
                    Point pos = e.GetPosition(canvas);
                    _preview.HandleEnd(pos.X, pos.Y);

                    // Xoá hết các hình vẽ cũ
                    canvas.Children.Clear();

                    // Vẽ lại các hình trước đó
                    for (int i = 0; i < _shapes.Count; i++)
                    {
                        if (images.ContainsKey(i))
                        {
                            foreach (var image in images[i])
                            {
                                canvas.Children.Add(image);
                            }
                        }

                        var element = _shapes[i].Draw();
                        canvas.Children.Add(element);
                    }

                    // Vẽ hình preview đè lên
                    canvas.Children.Add(_preview.Draw());

                }
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            _isDrawing = false;
            
            // Thêm đối tượng cuối cùng vào mảng quản lí
            Point pos = e.GetPosition(canvas);
            if (_preview != null)
            {
                _preview.HandleEnd(pos.X, pos.Y);

                _shapes.Add(_preview);
                if (_selectedShapeName == "")
                {
                    return;
                }
                // Sinh ra đối tượng mẫu kế
                _preview = _prototypes[_selectedShapeName].Clone();

                // Ve lai Xoa toan bo
                canvas.Children.Clear();

                // Ve lai tat ca cac hinh
                for (int i = 0; i < _shapes.Count; i++)
                {
                    if (images.ContainsKey(i))
                    {
                        foreach (var image in images[i])
                        {
                            canvas.Children.Add(image);
                        }
                    }
                    var element = _shapes[i].Draw();
                    canvas.Children.Add(element);
                }
            }
        }
        private int index = -1;
        private void methodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (methodComboBox.SelectedIndex != index)
            {

                KeyValuePair<string, IShape> selectedEntry = (KeyValuePair<string, IShape>)methodComboBox.SelectedItem;


                _selectedShapeName = selectedEntry.Key;


                if (_selectedShapeName == null)
                {
                    return;
                }
                _preview = _prototypes[_selectedShapeName];
                index = methodComboBox.SelectedIndex;
            }
        }

        private void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Load image";
            openFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                _preview = null;
                CreateLoadBitmap(ref canvas, openFileDialog.FileName);
            };
        }

        private void SaveImages_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            
            saveFileDialog.Filter = "Images|*.png";
            saveFileDialog.Title = "Save as PNG";
            saveFileDialog.RestoreDirectory = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                String fileName = saveFileDialog.FileName;
                CreateSaveBitmap(canvas, fileName);
            }
        }
        private void CreateLoadBitmap(ref Canvas canvas, string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();

            Image image = new Image();
            image.Source = bitmap;
            image.Width = bitmap.Width;
            image.Height = bitmap.Height;
            if (bitmap.Width > canvas.Width || double.IsNaN(canvas.Width))
            {
                canvas.Width = bitmap.Width > canvas.ActualWidth ? bitmap.Width : double.NaN;
            }
            if (bitmap.Height > canvas.Height || double.IsNaN(canvas.Height))
            {
                canvas.Height = bitmap.Height > canvas.ActualHeight ? bitmap.Height : double.NaN;
            }
            if (!images.ContainsKey(_shapes.Count))
            {
                images[_shapes.Count] = new List<Image>();
            }
            images[_shapes.Count].Add(image);
            canvas.Children.Add(image);
        }
        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
            canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));

            renderBitmap.Render(canvas);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }
    }
}
